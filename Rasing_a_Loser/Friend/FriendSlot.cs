using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//todo 텍스트 연결, 초반 수입 정원님한테 받으면 로직넣고 수정하기.
public class FriendSlot : MonoBehaviour
{
    public delegate void LevelChangedDelegate(FriendSlot slot, int newLevel);
    public event LevelChangedDelegate OnLevelChanged;

    public FriendInfo friendInfo; // 이 UI가 표현하는 친구 객체
    public double presentUpgradeCost;
    public int slotIndex;
    public int level;
    public TMP_Text nameText; // 이름, 레벨을 표시할 UI 컴포넌트
    public TMP_Text effectText; // 수입을 표시할 UI 컴포넌트
    public TMP_Text upgradeCost; // 업그레이드 비용을 표시할 UI 컴포넌트
    public TMP_Text stateText;
    public Button friendButton; // 이 친구를 해금하는 데 사용되는 버튼
    public Image profileImg;// 프로필 이미지

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
        // 친구가 아직 해금되지 않았다면
        if (!FriendManager.Instance.IsFriendUnlocked(this))
        {
            // 돈이 충분한지 검사하고, 앞선 친구가 10레벨인지 확인.
            if (CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, -friendInfo.unlockCost) && CheckBeforeFriendLevel())
            {
                FriendManager.Instance.UnlockFriend(this); // 친구 해금 데이터 추가
                TestUIManager.Instance.UpdateFriendListUI(slotIndex); // 친구 오브젝트 활성화
                UpgradeFriend();
                TestUIManager.Instance.friendEventUI.SetActive(true);
                EventChatManager.Instance.StartDialogue(friendInfo.friendName);
                friendButton.image.sprite = ResourceManager.Instance.GetResource<Sprite>("UpgradeBtn");
                profileImg.sprite = ResourceManager.Instance.GetResource<Sprite>(friendInfo.friendName);
            }
        }
        else
        {
            // 업그레이드 구현.
            // 아마 인맥 레벨 올려주면 될듯?
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
            effectText.text = $"<size=30><color=#FF0000>♥</color>{CalculateIncome()}/초</size>"; // 수입 정보는 30pt

        if (nameText != null)
            nameText.text = $"<size=35>{friendInfo.friendName}</size>\n" + // 이름은 35pt
                            $"<size=25>Lv. {level}</size>\n"; // 레벨은 25pt

        if (upgradeCost != null)
            upgradeCost.text = $"<color=#FF0000>♥</color> {CalculateUpgradeCost()}";
    }

    public double CalculateIncome()
    {
        // Todo : 증가율 로직 짜서 다시 적용하기.
        // 예: 기본 수입에 레벨을 곱하여 계산
        return friendInfo.moneyPerSecond * level;
    }

    private double CalculateUpgradeCost()
    {
        // Todo : 증가율 로직 짜서 다시 적용하기.
        // 예: 기본 업그레이드 비용에 레벨에 따른 증가율을 적용하여 계산
        return presentUpgradeCost * (1.08 + ((level - 1) * 0.0002));
    }

    public void UpgradeFriend()
    {
        level++;
        UpdateLevelUI();

        OnLevelChanged?.Invoke(this, level);
    }

    public bool CheckBeforeFriendLevel() // 앞선 친구 레벨 10인지 확인하기
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

        else return true;// 0일 때는 첫번째 친구라는 뜻이니, 무조건 true
    }
}
