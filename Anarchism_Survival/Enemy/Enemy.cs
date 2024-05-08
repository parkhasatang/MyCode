using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data; // ���� �����͸� ������ ScriptableObject
    public float currentHp; // ���� ���� ü��
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

    // ���� �������� �޴� �޼ҵ�
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void SetEnemyStats() //������ ����.
    {
        int playerLevel = GameManager.Instance.level; // �� ���� ���� ���� ������ ���� �÷��̾��� ���� ������ �����;� �մϴ�.
        currentHp = data.GetScaledHp(playerLevel); // �÷��̾��� ������ ���� ü�� ����
        exp = data.experienceReward;
        animator.runtimeAnimatorController = data.animatorController;
    }

    // �� ��� ó��
    void Die()
    {
        // �� ��� ����, ���� ��� ������Ʈ ��Ȱ��ȭ �Ǵ� �ı�
        gameObject.SetActive(false);
        GameObject dropItem = PoolManager.instance.GetPool(expPrefabId);// ����ġ Prefab
        dropItem.GetComponent<ExpCapsule>().exp = exp;
        dropItem.transform.position = transform.position;
    }
}