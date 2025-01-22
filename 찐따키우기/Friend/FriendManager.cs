using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    public static FriendManager Instance;

    public List<FriendSlot> friendSlots; // UIģ�� ���Ժ� ������
    public FriendDatabase friendDatabase; // ģ�� ������ ����� �����ͺ��̽�
    private List<FriendSlot> unlockedFriends = new List<FriendSlot>(); // �رݵ� ģ���� ���� ����

    internal double totalMoney;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;

        InitializeFriends();
    }

    // �ʴ� ���� �����ϴ� �Լ�
    public double GetTotalMoneyPerSecond()
    {
        // �ʱ�ȭ �ѹ�
        totalMoney = 0;

        foreach (var friend in unlockedFriends)
        {
            totalMoney += friend.CalculateIncome();
        }
        return totalMoney;
    }

    // ģ�� �ر�
    public void UnlockFriend(FriendSlot friend)
    {
        if (!unlockedFriends.Contains(friend))
        {
            unlockedFriends.Add(friend);
            // �߰� ����: �رݵ� ģ�� ������ �����ϰų� UI ������Ʈ ��
        }
    }

    // ģ���� �ر� ���� Ȯ��
    public bool IsFriendUnlocked(FriendSlot friend)
    {
        return unlockedFriends.Contains(friend);
    }

    private void InitializeFriends()
    {
        // friendDatabase���� ģ�� ������ �ҷ��� �� ���Կ� �Ҵ�
        for (int i = 0; i < friendSlots.Count && i < friendDatabase.friends.Count; i++)
        {
            friendSlots[i].InitializeSlot(friendDatabase.friends[i], i);
            if (i < friendSlots.Count - 1) // ������ ������ �ƴ� ��쿡�� �̺�Ʈ ����
            {
                friendSlots[i].OnLevelChanged += HandleLevelChange;
            }
        }
    }

    private void HandleLevelChange(FriendSlot slot, int newLevel)
    {
        // ���� ������ �ε����� ã��
        int currentIndex = friendSlots.IndexOf(slot);

        // ������ 10 �̻��̰�, ���� ������ �����ϴ� ���
        if (newLevel >= 10 && currentIndex >= 0 && currentIndex + 1 < friendSlots.Count)
        {
            // ���� ������ ��ư�� Ȱ��ȭ
            FriendSlot nextSlot = friendSlots[currentIndex + 1];
            if (nextSlot != null && nextSlot.friendButton != null)
            {
                nextSlot.nameText.text = $"<size=30>�ر�: {nextSlot.friendInfo.unlockCost}�ʿ�</size>";
                nextSlot.effectText.text = $"<size=30>???</size>";
                nextSlot.friendButton.interactable = true;
                nextSlot.friendButton.image.sprite = ResourceManager.Instance.GetResource<Sprite>("UnlockBtn");
                friendSlots[currentIndex].OnLevelChanged -= HandleLevelChange;
            }
        }
    }

    private void OnDestroy()
    {
        // ������ ������ �����ϰ� �̺�Ʈ ���� ����
        for (int i = 0; i < friendSlots.Count - 1; i++)
        {
            friendSlots[i].OnLevelChanged -= HandleLevelChange;
        }
    }
}
