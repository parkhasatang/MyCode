using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data; // 적의 데이터를 저장할 ScriptableObject
    public float currentHp; // 적의 현재 체력
    public float exp;
    public int expPrefabId;
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetEnemyStats();

    }

    // 적이 데미지를 받는 메소드
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void SetEnemyStats() //데이터 주입.
    {
        int playerLevel = GameManager.Instance.level; // 이 값은 실제 게임 로직에 따라 플레이어의 현재 레벨을 가져와야 합니다.
        currentHp = data.GetScaledHp(playerLevel); // 플레이어의 레벨에 따라 체력 조정
        exp = data.experienceReward;
        animator.runtimeAnimatorController = data.animatorController;
    }

    // 적 사망 처리
    void Die()
    {
        // 적 사망 로직, 예를 들어 오브젝트 비활성화 또는 파괴
        gameObject.SetActive(false);
        GameObject dropItem = PoolManager.instance.GetPool(expPrefabId);// 경험치 Prefab
        dropItem.GetComponent<ExpCapsule>().exp = exp;
        dropItem.transform.position = transform.position;
    }
}