using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    public static FriendManager Instance;

    public List<FriendSlot> friendSlots; // UI친구 슬롯별 데이터
    public FriendDatabase friendDatabase; // 친구 정보가 저장된 데이터베이스
    private List<FriendSlot> unlockedFriends = new List<FriendSlot>(); // 해금된 친구들 정보 저장

    internal double totalMoney;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;

        InitializeFriends();
    }

    // 초당 돈을 생성하는 함수
    public double GetTotalMoneyPerSecond()
    {
        // 초기화 한번
        totalMoney = 0;

        foreach (var friend in unlockedFriends)
        {
            totalMoney += friend.CalculateIncome();
        }
        return totalMoney;
    }

    // 친구 해금
    public void UnlockFriend(FriendSlot friend)
    {
        if (!unlockedFriends.Contains(friend))
        {
            unlockedFriends.Add(friend);
            // 추가 로직: 해금된 친구 정보를 저장하거나 UI 업데이트 등
        }
    }

    // 친구의 해금 여부 확인
    public bool IsFriendUnlocked(FriendSlot friend)
    {
        return unlockedFriends.Contains(friend);
    }

    private void InitializeFriends()
    {
        // friendDatabase에서 친구 정보를 불러와 각 슬롯에 할당
        for (int i = 0; i < friendSlots.Count && i < friendDatabase.friends.Count; i++)
        {
            friendSlots[i].InitializeSlot(friendDatabase.friends[i], i);
            if (i < friendSlots.Count - 1) // 마지막 슬롯이 아닌 경우에만 이벤트 구독
            {
                friendSlots[i].OnLevelChanged += HandleLevelChange;
            }
        }
    }

    private void HandleLevelChange(FriendSlot slot, int newLevel)
    {
        // 현재 슬롯의 인덱스를 찾음
        int currentIndex = friendSlots.IndexOf(slot);

        // 레벨이 10 이상이고, 다음 슬롯이 존재하는 경우
        if (newLevel >= 10 && currentIndex >= 0 && currentIndex + 1 < friendSlots.Count)
        {
            // 다음 슬롯의 버튼을 활성화
            FriendSlot nextSlot = friendSlots[currentIndex + 1];
            if (nextSlot != null && nextSlot.friendButton != null)
            {
                nextSlot.nameText.text = $"<size=30>해금: {nextSlot.friendInfo.unlockCost}필요</size>";
                nextSlot.effectText.text = $"<size=30>???</size>";
                nextSlot.friendButton.interactable = true;
                nextSlot.friendButton.image.sprite = ResourceManager.Instance.GetResource<Sprite>("UnlockBtn");
                friendSlots[currentIndex].OnLevelChanged -= HandleLevelChange;
            }
        }
    }

    private void OnDestroy()
    {
        // 마지막 슬롯을 제외하고 이벤트 구독 해제
        for (int i = 0; i < friendSlots.Count - 1; i++)
        {
            friendSlots[i].OnLevelChanged -= HandleLevelChange;
        }
    }
}
