using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int baseHp;
    public int attackDamage;
    public float experienceReward;
    public RuntimeAnimatorController animatorController;

    // 플레이어의 레벨에 따라 적의 체력을 조정하는 메소드
    public int GetScaledHp(int playerLevel)
    {
        return baseHp + ((playerLevel - 1) * 10); // 예: 레벨이 높아질 때마다 체력을 10씩 증가
    }
}
