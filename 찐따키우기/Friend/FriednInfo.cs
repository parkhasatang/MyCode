using UnityEngine;

[CreateAssetMenu(fileName = "FriendInfo", menuName = "Scriptable Object/Friend Info")]
public class FriendInfo : ScriptableObject
{
    public string friendName;
    public double unlockCost; // 이 친구를 구매하는 데 필요한 돈의 양
    public double upgradeCost; // 업그레이드 비용
    public double moneyPerSecond; // 초당 생성하는 돈의 양
}