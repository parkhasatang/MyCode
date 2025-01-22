using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 증분 처리를 위한 데이터 구조체
public struct IncrementData
{
    public int startValue;
    public int increaseValue;
    public int maxValue;
    public int remainPoints;

    public IncrementData(int startValue, int increaseValue, int maxValue, int remainPoints)
    {
        this.startValue = startValue;
        this.increaseValue = increaseValue;
        this.maxValue = maxValue;
        this.remainPoints = remainPoints;
    }
}

public class AchievementUIView : UIView
{
    public RectTransform starPosition;
    public Slider questProgressBar; // 칭호 진행 바
    public TMP_Text currentProgressCountText; // 칭호 진행도
    public TMP_Text titleText; // 현재 칭호 표시용

    public RectTransform achievementSlotContentTransform; // AchievementSlot이 들어갈 Content
    
    private Tween progressBarTween;
    private AchievementManager achievementManager;
    
    // 증가 요청을 담는 큐
    private Queue<IncrementData> incrementQueue = new Queue<IncrementData>();
    private bool isAnimating = false; // 현재 애니메이션 처리 중인지 여부

    protected override void Awake()
    {
        base.Awake();
        achievementManager = AchievementManager.Instance;
        if (achievementManager != null)
        {
            UpdateTitle(achievementManager.currentAchievementTitle.titleName);
        }
        else
        {
            Debug.LogError("업적 타이틀 정보를 가져올 수 없습니다.");
        }

        if (backButtons != null)
        {
            foreach (var button in backButtons)
            {
                button.onClick.AddListener(() => GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true));
            }
        }
    }

    public void UpdateTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = $"[{title}]";
        }
    }
    
    public void UpdateCurrentStarCountText()
    {
        currentProgressCountText.text = $"{achievementManager.currentAchievementTitle.currentPoints}/{achievementManager.currentAchievementTitle.requiredPoints}";
    }
    
    private void AnimateIncrement(int startValue, int increaseValue, int maxValue, int remainPoints)
    {
        isAnimating = true;

        // startValue를 기준으로 증가
        int targetValue = startValue + increaseValue;
        int intermediateTarget = Mathf.Min(targetValue, maxValue);
        float duration = 0.5f;
        int currentPoints = startValue;

        // 기존 Tween이 있으면 종료
        if (progressBarTween != null && progressBarTween.IsActive())
        {
            progressBarTween.Kill();
            progressBarTween = null;
        }

        var seq = DOTween.Sequence();

        // 1단계: 현재 칭호의 최대치까지 증가
        seq.Append(
            DOTween.To(
                () => currentPoints,
                x =>
                {
                    currentPoints = x;
                    float progressRatio = maxValue > 0 ? Mathf.Clamp01((float)currentPoints / maxValue) : 0f;
                    questProgressBar.value = progressRatio;
                    currentProgressCountText.text = $"{currentPoints}/{maxValue}";
                },
                intermediateTarget,
                duration
            ).SetEase(Ease.OutCubic)
        );

        // maxValue를 초과했다면 다음 칭호로 넘어가기
        if (targetValue >= maxValue)
        {
            seq.AppendCallback(() =>
            {
                // 현재 칭호 변경 전 oldTitle 저장
                string oldTitle = achievementManager.currentAchievementTitle.titleName;
                
                // 다음 칭호로 변경
                achievementManager.SetNextAchievementTitle(achievementManager.currentAchievementTitle.level + 1);
                
                var zoneUI = UIManager.Instance.GetUIView<AchievementZoneUIView>("AchievementZoneUI");
    
                // 새 칭호
                string newTitle = achievementManager.currentAchievementTitle.titleName;

                // achievementTitlesDictionary에서 랜덤 칭호 4개 가져오기 (oldTitle, newTitle 제외)
                string[] randomTitles = GetRandomTitlesExcluding(oldTitle, newTitle, 4);

                // 슬롯머신에 쓸 문자열 배열 (총 4개: old, random1, random2, newTitle)
                // randomTitles가 2개라고 가정
                string[] slotMachineTitles = new string[4];
                slotMachineTitles[0] = randomTitles[0];
                slotMachineTitles[1] = randomTitles[1];
                slotMachineTitles[2] = randomTitles[2];
                slotMachineTitles[3] = randomTitles[3];

                UIManager.Instance.ShowUI("AchievementZoneUI");
                // 슬롯머신 시작
                zoneUI.titleSlotMachine.StartSlotMachine(slotMachineTitles, newTitle);

                int newMaxValue = achievementManager.currentAchievementTitle.requiredPoints;
                int newCurrentPoints = 0; // 다음 칭호는 경험치 0부터
                questProgressBar.value = 0f;
                currentProgressCountText.text = $"0/{newMaxValue}";

                // remainPoints만큼 추가로 증가
                if (remainPoints > 0)
                {
                    seq.Append(
                        DOTween.To(
                            () => newCurrentPoints,
                            x =>
                            {
                                newCurrentPoints = x;
                                float newProgressRatio = newMaxValue > 0 ? Mathf.Clamp01((float)newCurrentPoints / newMaxValue) : 0f;
                                questProgressBar.value = newProgressRatio;
                                currentProgressCountText.text = $"{newCurrentPoints}/{newMaxValue}";
                            },
                            remainPoints,
                            duration
                        ).SetEase(Ease.OutCubic)
                    );
                }
            });
        }

        seq.OnComplete(() =>
        {
            isAnimating = false;
            TryProcessNextIncrement();
        });

        progressBarTween = seq;
    }

    public void EnqueueTitleProgressBarUpdate(int startValue, int increaseValue, int maxValue, int remainPoints)
    {
        incrementQueue.Enqueue(new IncrementData(startValue, increaseValue, maxValue, remainPoints));
        TryProcessNextIncrement();
    }
    
    private void TryProcessNextIncrement()
    {
        if (isAnimating || incrementQueue.Count == 0) return;

        var increment = incrementQueue.Dequeue();
        AnimateIncrement(increment.startValue, increment.increaseValue, increment.maxValue, increment.remainPoints);
    }
    
    private string[] GetRandomTitlesExcluding(string oldTitle, string newTitle, int randomCount)
    {
        List<AchievementTitle> allTitles = new List<AchievementTitle>(achievementManager.achievementTitlesDictionary.Values);

        // 제외할 제목 필터링
        allTitles.RemoveAll(t => t.titleName == oldTitle || t.titleName == newTitle);

        // Shuffle
        System.Random rng = new System.Random();
        for (int i = allTitles.Count - 1; i > 0; i--)
        {
            int k = rng.Next(i + 1);
            (allTitles[i], allTitles[k]) = (allTitles[k], allTitles[i]);
        }

        // 앞에서부터 randomCount개 추출
        string[] result = new string[randomCount];
        for (int i = 0; i < randomCount; i++)
        {
            result[i] = allTitles[i].titleName;
        }

        return result;
    }
}
