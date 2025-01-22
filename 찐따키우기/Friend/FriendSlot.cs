using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//todo �ؽ�Ʈ ����, �ʹ� ���� ���������� ������ �����ְ� �����ϱ�.
public class FriendSlot : MonoBehaviour
{
    public delegate void LevelChangedDelegate(FriendSlot slot, int newLevel);
    public event LevelChangedDelegate OnLevelChanged;

    public FriendInfo friendInfo; // �� UI�� ǥ���ϴ� ģ�� ��ü
    public double presentUpgradeCost;
    public int slotIndex;
    public int level;
    public TMP_Text nameText; // �̸�, ������ ǥ���� UI ������Ʈ
    public TMP_Text effectText; // ������ ǥ���� UI ������Ʈ
    public TMP_Text upgradeCost; // ���׷��̵� ����� ǥ���� UI ������Ʈ
    public TMP_Text stateText;
    public Button friendButton; // �� ģ���� �ر��ϴ� �� ���Ǵ� ��ư
    public Image profileImg;// ������ �̹���

    public void InitializeSlot(FriendInfo friendInfo, int index)
    {
        this.friendInfo = friendInfo;
        this.slotIndex = index;
        this.presentUpgradeCost = friendInfo.upgradeCost;
    }
    private void Start()
    {
        FriendManager.Instance.GetTotalMoneyPerSecond();

        friendButton.onClick.AddListener(FriendSlotBtnClick);
    }

    public void FriendSlotBtnClick()
    {
        // ģ���� ���� �رݵ��� �ʾҴٸ�
        if (!FriendManager.Instance.IsFriendUnlocked(this))
        {
            // ���� ������� �˻��ϰ�, �ռ� ģ���� 10�������� Ȯ��.
            if (CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, -friendInfo.unlockCost) && CheckBeforeFriendLevel())
            {
                FriendManager.Instance.UnlockFriend(this); // ģ�� �ر� ������ �߰�
                TestUIManager.Instance.UpdateFriendListUI(slotIndex); // ģ�� ������Ʈ Ȱ��ȭ
                UpgradeFriend();
                TestUIManager.Instance.friendEventUI.SetActive(true);
                EventChatManager.Instance.StartDialogue(friendInfo.friendName);
                friendButton.image.sprite = ResourceManager.Instance.GetResource<Sprite>("UpgradeBtn");
                profileImg.sprite = ResourceManager.Instance.GetResource<Sprite>(friendInfo.friendName);
            }
        }
        else
        {
            // ���׷��̵� ����.
            // �Ƹ� �θ� ���� �÷��ָ� �ɵ�?
            if (CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, -CalculateUpgradeCost()))
            {
                UpgradeFriend();
            }
        }

        FriendManager.Instance.GetTotalMoneyPerSecond();
    }

    private void UpdateLevelUI()
    {
        if (effectText != null)
            effectText.text = $"<size=30><color=#FF0000>��</color>{CalculateIncome()}/��</size>"; // ���� ������ 30pt

        if (nameText != null)
            nameText.text = $"<size=35>{friendInfo.friendName}</size>\n" + // �̸��� 35pt
                            $"<size=25>Lv. {level}</size>\n"; // ������ 25pt

        if (upgradeCost != null)
            upgradeCost.text = $"<color=#FF0000>��</color> {CalculateUpgradeCost()}";
    }

    public double CalculateIncome()
    {
        // Todo : ������ ���� ¥�� �ٽ� �����ϱ�.
        // ��: �⺻ ���Կ� ������ ���Ͽ� ���
        return friendInfo.moneyPerSecond * level;
    }

    private double CalculateUpgradeCost()
    {
        // Todo : ������ ���� ¥�� �ٽ� �����ϱ�.
        // ��: �⺻ ���׷��̵� ��뿡 ������ ���� �������� �����Ͽ� ���
        return presentUpgradeCost * (1.08 + ((level - 1) * 0.0002));
    }

    public void UpgradeFriend()
    {
        level++;
        UpdateLevelUI();

        OnLevelChanged?.Invoke(this, level);
    }

    public bool CheckBeforeFriendLevel() // �ռ� ģ�� ���� 10���� Ȯ���ϱ�
    {
        if (slotIndex != 0)
        {
            int beforeFriendIndex = slotIndex - 1;
            int beforeFriendLevel = FriendManager.Instance.friendSlots[beforeFriendIndex].level;
            if (beforeFriendLevel >= 10)
            {
                return true;
            }
            else return false;
        }

        else return true;// 0�� ���� ù��° ģ����� ���̴�, ������ true
    }
}
