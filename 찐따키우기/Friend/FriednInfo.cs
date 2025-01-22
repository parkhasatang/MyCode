using UnityEngine;

[CreateAssetMenu(fileName = "FriendInfo", menuName = "Scriptable Object/Friend Info")]
public class FriendInfo : ScriptableObject
{
    public string friendName;
    public double unlockCost; // �� ģ���� �����ϴ� �� �ʿ��� ���� ��
    public double upgradeCost; // ���׷��̵� ���
    public double moneyPerSecond; // �ʴ� �����ϴ� ���� ��
}