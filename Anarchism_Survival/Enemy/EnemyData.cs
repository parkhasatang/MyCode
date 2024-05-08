using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int baseHp;
    public int attackDamage;
    public float experienceReward;
    public RuntimeAnimatorController animatorController;

    // �÷��̾��� ������ ���� ���� ü���� �����ϴ� �޼ҵ�
    public int GetScaledHp(int playerLevel)
    {
        return baseHp + ((playerLevel - 1) * 10); // ��: ������ ������ ������ ü���� 10�� ����
    }
}
