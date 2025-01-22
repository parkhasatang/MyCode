using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardUIController : UIController
{
    public Button claimButton;
    public Button doubleRewardButton;

    public MilestoneReward currentMilestoneReward;
    private QuestUIController questUIController;
    private QuestRewardUIView questRewardUIView;

    private void OnEnable()
    {
        UIManager.Instance.uiEscapeStack.isEscapeLock = true;
    }

    private void Awake()
    {
        questRewardUIView = GetComponent<QuestRewardUIView>();
        // QuestUI가 무조건 먼저 생성되어서 UIManager에서 가져올 수 있음.
        questUIController = UIManager.Instance.GetUIController<QuestUIController>("QuestUI");
        
        // 람다를 사용하여 isAdvertising 값을 전달
        claimButton.onClick.AddListener(() => OnClaimButtonClicked(isAdvertising: false));
        claimButton.onClick.AddListener(() => SoundManager.Instance.PlaySoundEffect("퀘스트 보상"));
        doubleRewardButton.onClick.AddListener(() => OnClaimButtonClicked(isAdvertising: true));
        doubleRewardButton.onClick.AddListener(() => SoundManager.Instance.PlaySoundEffect("퀘스트 보상"));
    }

    public void SetReward(MilestoneReward milestoneReward)
    {
        currentMilestoneReward = milestoneReward;

        questRewardUIView.UpdateRewardUI(currentMilestoneReward);
        
        claimButton.interactable = true;
        doubleRewardButton.interactable = true;
    }

    private void OnClaimButtonClicked(bool isAdvertising)
    {
        UIManager.Instance.upperZone.transform.SetAsLastSibling();
    
        if (isAdvertising)
        {
            // 광고 시청 후 보상 두 배 지급
            if (AdManager.Instance.ShowAdReward())
            {
                OnAdCompleted(true);
            }

            // 광고 시청 중 버튼 비활성화
            claimButton.interactable = false;
            doubleRewardButton.interactable = false;
        }
        else
        {
            // 이펙트 파티클 생성
            SpawnEffectParticles(isDouble: false);
        }
    }

    private void OnAdCompleted(bool success)
    {
        if (success)
        {
            // 이펙트 파티클 생성
            SpawnEffectParticles(isDouble: true);
        }
        else
        {
            // 광고 시청 실패 시 버튼을 다시 활성화할 수 있음
            claimButton.interactable = true;
            doubleRewardButton.interactable = true;
        }
    }

    private void GrantRewards(bool isDouble)
    {
        // 보상 지급
        questUIController.GrantMilestoneRewards(currentMilestoneReward, isDouble);
        currentMilestoneReward.IsClaimed = true;
    
        // UI 닫기
        UIManager.Instance.HideUI("QuestRewardUI");

        // 데이터 리셋
        ResetData();
    
        if (!QuestManager.Instance.CheckQuestRedDot())
        {
            QuestManager.Instance.questRedDotComponent.OnRedDotChecked();
        }
    }

    private void ResetData()
    {
        currentMilestoneReward = null;
    }

    private void SpawnEffectParticles(bool isDouble)
    {
        // RectTransform을 사용하여 오브젝트의 중심 좌표 계산
        var rectTransform = isDouble ? doubleRewardButton.GetComponent<RectTransform>() : claimButton.GetComponent<RectTransform>();
        var worldPosition = rectTransform.TransformPoint(rectTransform.rect.center);

        // Rewards의 갯수 만큼 파티클 생성
        for (int i = 0; i < currentMilestoneReward.Rewards.Count; i++)
        {
            bool isLastParticle = (i == currentMilestoneReward.Rewards.Count - 1);

            // 클로저 문제를 해결하기 위해 지역 변수로 캡처
            var reward = currentMilestoneReward.Rewards[i];

            // 각 보상마다 새로운 파티클 시스템 생성
            CurrencyEffectParticleController ps = GameManager.Instance.Pool.SpawnFromPool<CurrencyEffectParticleController>(E_PoolObjectType.CurrencyEffect);
            ps.transform.position = worldPosition;

            // 마지막 파티클의 콜백에서만 보상 지급
            ps.InitParticle(reward.rewardType, true, true, 6, () => 
            {
                if (isLastParticle)
                {
                    GrantRewards(isDouble);
                }
            },null);
        }

        // 보상 수령 중 버튼 비활성화
        claimButton.interactable = false;
        doubleRewardButton.interactable = false;
    }

    private void OnDisable()
    {
        UIManager.Instance.uiEscapeStack.isEscapeLock = false;
    }
}
