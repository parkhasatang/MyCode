using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random; // DOTween 네임스페이스 추가

public class CurrencyEffectParticleController : PoolObject
{
    public Transform target; // 파티클들이 날아갈 목표 위치

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private bool hasFirstParticleReached = false;

    public float minSpeed; // 최소 속도
    public float maxSpeed; // 최대 속도
    public float stopDistance; // 목표 위치에 도달했다고 판단할 거리

    private float[] particleSpeeds; // 각 파티클의 속도 저장
    private float[] particleDelays; // 각 파티클의 지연 시간 저장
    private float moveStartTime;

    private bool isMoving;

    // 초기 상태 저장을 위한 변수
    private float originalGravityModifier;
    private bool originalLimitVelocityEnabled;
    private float originalLimitVelocity;

    private Tween targetScaleTween; // `target`의 스케일 애니메이션을 위한 Tween 변수
    private readonly Vector3 originalTargetScale = new Vector3(1, 1, 1); // 타겟의 원래 스케일

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Debug.LogError("ParticleSystem을 찾을 수 없습니다.");
            return;
        }

        // 초기 설정 값 저장
        SaveOriginalSettings();
    }

    public void InitParticle(CurrencyType currency, bool isInitOnUI, bool isSoundOn, long particleCount, Action onFirstParticleReached = null, Action onAllComplete = null, float particleSize = 1f)
    {
        var currencyEffectData = GameDataManager.Instance.GetCurrencyEffectData(currency);
        if (currencyEffectData != null)
        {
            var psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            psRenderer.material = currencyEffectData.currencyMaterial;
            target = currencyEffectData.currencyTransform;

            // Sorting Layer 설정
            if (isInitOnUI)
            {
                psRenderer.sortingLayerID = SortingLayer.NameToID("UI_Effect");
                psRenderer.sortingOrder = 40;
            }
            else
            {
                psRenderer.sortingLayerID = SortingLayer.NameToID("Default");
                psRenderer.sortingOrder = -500;
            }
            
            // 필요한 변수 초기화
            isMoving = false;
            hasFirstParticleReached = false; // 첫 파티클 도착 여부 초기화

            // 파티클 시스템의 Emission 모듈에서 파티클 개수 설정
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0.0f, (short)particleCount)
            });

            var main = ps.main;
            ParticleSystem.MinMaxCurve originalSize = main.startSize;
            ParticleSystem.MinMaxCurve newSize = new ParticleSystem.MinMaxCurve(
                originalSize.constant * particleSize
            );
            main.startSize = newSize;

            ps.Clear();
            ps.Play();

            main.startSize = originalSize;
            // 파티클 이동 코루틴 시작 (1초 후에 이동 시작)
            StartCoroutine(StartMovingParticles(1f, isSoundOn, onFirstParticleReached, onAllComplete, currency));
        }
        else
        {
            Debug.LogWarning($"{currency}와 일치하는 Material이 존재하지 않습니다.");
        }
    }

    private void SaveOriginalSettings()
    {
        var mainModule = ps.main;
        originalGravityModifier = mainModule.gravityModifier.constant;

        var limitVelocityModule = ps.limitVelocityOverLifetime;
        originalLimitVelocityEnabled = limitVelocityModule.enabled;
        originalLimitVelocity = limitVelocityModule.limitMultiplier;
    }

    private IEnumerator StartMovingParticles(float delay, bool isSoundOn, Action onFirstParticleReached, Action onAllComplete, CurrencyType currency)
    {
        yield return new WaitForSeconds(delay);

        // 파티클 시스템 설정 변경
        DisableLimitVelocityAndGravity();

        int numParticlesAlive = ps.particleCount;
        InitializeParticleSpeeds(numParticlesAlive); // 각 파티클의 속도 초기화
        InitializeParticleDelays(numParticlesAlive); // 각 파티클의 지연 시간 초기화

        moveStartTime = Time.time; // 이동 시작 시간 기록

        isMoving = true;

        // 파티클 이동 코루틴 시작
        StartCoroutine(MoveParticlesCoroutine(onFirstParticleReached, isSoundOn, currency));

        // 이펙트 완료를 감시하는 코루틴 시작
        StartCoroutine(WaitForEffectCompletion(onAllComplete));
    }

    private IEnumerator MoveParticlesCoroutine(Action onFirstParticleReached, bool isSoundOn, CurrencyType currency)
    {
        while (isMoving)
        {
            MoveParticlesToTarget(onFirstParticleReached, isSoundOn, currency);
            yield return null; // 다음 프레임까지 대기
        }
    }

    private IEnumerator WaitForEffectCompletion(Action onComplete)
    {
        // 파티클이 남아 있는 동안 대기
        while (ps.particleCount > 0)
        {
            yield return null;
        }

        ps.Stop();

        // 초기화
        isMoving = false;

        // 원래의 설정 값 복원
        RestoreOriginalSettings();

        // 콜백 함수 호출
        onComplete?.Invoke();

        // 오브젝트를 풀로 반환
        GameManager.Instance.Pool.ReturnToPool(E_PoolObjectType.CurrencyEffect, this);
    }

    private void DisableLimitVelocityAndGravity()
    {
        // Gravity Modifier 값을 0으로 설정
        var mainModule = ps.main;
        originalGravityModifier = mainModule.gravityModifier.constant;
        mainModule.gravityModifier = 0f;

        // Limit Velocity over Lifetime 모듈 비활성화
        var limitVelocityModule = ps.limitVelocityOverLifetime;
        originalLimitVelocityEnabled = limitVelocityModule.enabled;
        originalLimitVelocity = limitVelocityModule.limitMultiplier;
        limitVelocityModule.enabled = false;
    }

    private void RestoreOriginalSettings()
    {
        var mainModule = ps.main;
        mainModule.gravityModifier = originalGravityModifier;

        var limitVelocityModule = ps.limitVelocityOverLifetime;
        limitVelocityModule.enabled = originalLimitVelocityEnabled;
        limitVelocityModule.limitMultiplier = originalLimitVelocity;
    }

    private void InitializeParticleSpeeds(int numParticles)
    {
        particleSpeeds = new float[numParticles];

        for (int i = 0; i < numParticles; i++)
        {
            // 원하는 범위의 랜덤 속도 설정
            particleSpeeds[i] = Random.Range(minSpeed, maxSpeed);
        }
    }

    private void InitializeParticleDelays(int numParticles)
    {
        particleDelays = new float[numParticles];

        for (int i = 0; i < numParticles; i++)
        {
            // 원하는 범위의 랜덤 지연 시간 설정
            particleDelays[i] = Random.Range(0f, 0.5f);
        }
    }

    private void MoveParticlesToTarget(Action onFirstParticleReached, bool isSoundOn, CurrencyType currency)
    {
        int numParticlesAlive = ps.particleCount;

        if (particles == null || particles.Length < ps.main.maxParticles)
        {
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        ps.GetParticles(particles);

        float numPenalty = Mathf.Sqrt(1 / (float)ps.particleCount);
        
        for (int i = 0; i < numParticlesAlive; i++)
        {
            // 파티클별로 지연 시간 체크
            float elapsedTime = Time.time - moveStartTime;

            if (elapsedTime >= particleDelays[i])
            {
                // 목표 위치
                Vector3 currentPos = particles[i].position;
                Vector3 targetPos = target.position;
                
                // 각 파티클의 고유한 속도를 사용하여 프레임 독립적으로 이동
                float speed = particleSpeeds[i];
                Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
                
                particles[i].position = newPos;
                // velocity는 MoveTowards 사용 시 별도로 필요 없으므로 0으로 설정
                particles[i].velocity = Vector3.zero;

                // 목표 위치에 도달했는지 체크
                float distanceToTarget = Vector3.Distance(particles[i].position, target.position);

                if (distanceToTarget <= stopDistance)
                {
                    // 파티클의 남은 수명을 0으로 설정하여 사라지게 함
                    particles[i].remainingLifetime = 0f;

                    // 타겟 스케일 애니메이션 실행
                    PlayTargetScaleAnimation();

                    // 사운드 실행
                    if (isSoundOn)
                    {
                        if (currency == CurrencyType.Money || currency == CurrencyType.AdRemovalTicket)
                        {
                            SoundManager.Instance.PlaySoundEffect("재화 이펙트", 1f * numPenalty, 0.7f);
                        }
                        else
                        {
                            SoundManager.Instance.PlaySoundEffect("진주", 1f * numPenalty, 0.7f);
                        }
                    }

                    // **첫 번째 파티클 도착 시 콜백 호출**
                    if (!hasFirstParticleReached)
                    {
                        hasFirstParticleReached = true;
                        onFirstParticleReached?.Invoke();
                    }
                }
            }
            else
            {
                // 지연 시간 동안 파티클을 정지시킴
                particles[i].velocity = Vector3.zero;
            }
        }

        ps.SetParticles(particles, numParticlesAlive);
    }

    private void PlayTargetScaleAnimation()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null in PlayTargetScaleAnimation()");
            return;
        }

        // 기존 Tween을 종료하고 타겟의 스케일을 원래대로 복원
        target.DOKill();
        target.localScale = originalTargetScale;

        // 스케일 애니메이션 실행 (1.2배로 커졌다가 원래 크기로 돌아옴)
        target.DOScale(originalTargetScale * 1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
