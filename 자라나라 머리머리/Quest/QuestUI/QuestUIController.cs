using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestUIController : UIController
{
    public Material starMaterial;
    
    [HideInInspector]
    public QuestUIView questUIView;

    private int CurrentStarCount
    {
        get => QuestManager.Instance.currentStarCount; // 현재 별 수량
        set => QuestManager.Instance.currentStarCount = value;
    }

    private int totalStarCount = 100;  // 전체 목표 별 수량

    public List<QuestRewardBox> questRewardBoxes;
    private List<QuestSlot> questSlots = new();
    private List<MilestoneReward> milestoneRewards = new();

    private void Awake()
    {
        questUIView = GetComponent<QuestUIView>();
        
        // 보상 데이터를 QuestManager에서 가져옵니다.
        milestoneRewards = QuestManager.Instance.MilestoneRewards;
    }

    private void Start()
    {
        // 게임 시작 후에 데이터를 추가할 때
        CurrencyEffectData newCurrencyData = new CurrencyEffectData
        {
            currencyType = CurrencyType.QuestStar,
            currencyMaterial = starMaterial,
            currencyTransform = questUIView.starTransform
        };

        GameDataManager.Instance.AddCurrencyEffectData(newCurrencyData);

        GenerateQuestSlots();
        InitializeRewardBoxes();
        LoadStarData();
        CheckQuestRewardMilestones();
        UpdateQuestSlotOrder();
        
        if (questUIView.backButtons != null)
        {
            foreach (var button in questUIView.backButtons)
            {
                button.onClick.AddListener(() => SetQuestUIInteractable(false));
            }
        }
    }

    private void GenerateQuestSlots()
    {
        List<Quest> quests = QuestManager.Instance.Quests;

        for (int i = 0; i < quests.Count; i++)
        {
            CreateQuestSlot(quests[i], i);
        }
        
        questUIView.questSlotContentTransform.localPosition = Vector3.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(questUIView.questSlotContentTransform);
    }

    private void CreateQuestSlot(Quest quest, int index)
    {
        GameObject questSlotPrefab = ResourceManager.Instance.GetResource<GameObject>("QuestSlot");
        GameObject questSlotInstance = Instantiate(questSlotPrefab, questUIView.questSlotContentTransform);
        QuestSlot questSlot = questSlotInstance.GetComponent<QuestSlot>();
        questSlot.Initialize(quest);

        // 리스트에 요소 추가
        questSlots.Add(questSlot);

        questSlotInstance.transform.SetSiblingIndex(index);
    }

    public void UpdateQuestSlotOrder()
    {
        // 완료 가능한 슬롯과 완료된 슬롯을 분류
        List<QuestSlot> completableSlots = new List<QuestSlot>();
        List<QuestSlot> completedSlots = new List<QuestSlot>();
        List<QuestSlot> inProgressSlots = new List<QuestSlot>();

        foreach (var slot in questSlots)
        {
            if (completableSlots.Contains(slot) || completedSlots.Contains(slot) || inProgressSlots.Contains(slot))
            {
                // 슬롯이 이미 리스트 중 하나에 포함되어 있으면 건너뜀
                continue;
            }

            if (slot.Quest.IsCompleted && !slot.Quest.isRewardClaimed)
            {
                completableSlots.Add(slot);
            }
            else if (slot.Quest.isRewardClaimed)
            {
                completedSlots.Add(slot);
            }
            else
            {
                inProgressSlots.Add(slot);
            }
        }

        // 슬롯 순서를 재정렬 (상단: 완료 가능한 슬롯, 중간: 진행 중인 슬롯, 하단: 완료된 슬롯)
        int index = 0;

        foreach (var slot in completableSlots)
        {
            slot.transform.SetSiblingIndex(index++);
        }

        foreach (var slot in inProgressSlots)
        {
            slot.transform.SetSiblingIndex(index++);
        }

        foreach (var slot in completedSlots)
        {
            slot.transform.SetSiblingIndex(index++);
        }

        Debug.Log("퀘스트 슬롯 정렬 완료");
    }
    
    private void InitializeRewardBoxes()
    {
        // `questRewardBoxes` 리스트와 `milestoneRewards` 리스트의 크기가 일치하는지 확인
        if (questRewardBoxes.Count != milestoneRewards.Count)
        {
            Debug.LogError("QuestRewardBox의 수와 MilestoneRewards의 수가 일치하지 않습니다.");
            return;
        }

        for (int i = 0; i < questRewardBoxes.Count; i++)
        {
            questRewardBoxes[i].Initialize(milestoneRewards[i], this);
        }
    }

    private void LoadStarData()
    {
        questUIView.UpdateCurrentStarCountText(CurrentStarCount);
        float progressRatio = totalStarCount > 0 ? (float)CurrentStarCount / totalStarCount : 0f;
        questUIView.questProgressBar.value = progressRatio;
        
        Debug.Log("별 수 로드 완료");
    }

    public void AddStarCount(int amount)
    {
        CurrentStarCount += amount;
        if (CurrentStarCount >= totalStarCount)
        {
            CurrentStarCount = totalStarCount;
        }
    }

    public void DisPlayStarCount()
    {
        questUIView.UpdateStarUI(CurrentStarCount, totalStarCount);
    }

    public void CheckQuestRewardMilestones()
    {
        for (int i = 0; i < milestoneRewards.Count; i++)
        {
            var milestone = milestoneRewards[i];
            var questRewardBox = questRewardBoxes[i];

            if (!milestone.IsClaimed)
            {
                if (CurrentStarCount >= milestone.StarRequirement)
                {
                    // 보상을 받을 수 있는 상태로 설정
                    questRewardBox.SetAvailable();
                }
                else
                {
                    // 아직 보상을 받을 수 없는 상태
                    questRewardBox.SetUnavailable();
                }
            }
            else
            {
                // 이미 보상을 받은 경우
                questRewardBox.SetClaimed();
            }
        }
    }

    public void RewardBoxClicked(QuestRewardBox questRewardBox, MilestoneReward milestoneReward)
    {
        if (milestoneReward != null && !milestoneReward.IsClaimed && CurrentStarCount >= milestoneReward.StarRequirement)
        {
            // 보상 수령 UI 생성
            UIManager.Instance.ShowUI("QuestRewardUI");
            QuestRewardUIController questRewardUIController = UIManager.Instance.GetUIController<QuestRewardUIController>("QuestRewardUI");
            questRewardUIController.SetReward(milestoneReward);
        }
        // 보상 미리보기 생성
        else
        {
            var boxRewardNoticeUIController = UIManager.Instance.GetUIController<BoxRewardNoticeUIController>("BoxRewardNoticeUI");

            if (boxRewardNoticeUIController == null)
            {
                UIManager.Instance.ShowUI("BoxRewardNoticeUI");
                boxRewardNoticeUIController = UIManager.Instance.GetUIController<BoxRewardNoticeUIController>("BoxRewardNoticeUI");
            }
            else
            {
                if (boxRewardNoticeUIController.gameObject.activeInHierarchy)
                {
                    boxRewardNoticeUIController.gameObject.SetActive(false);
                } 
                UIManager.Instance.ShowUI("BoxRewardNoticeUI");
            }
            
            // 위치 설정 메서드 호출
            SetBoxRewardNoticeUIPosition(questRewardBox, boxRewardNoticeUIController);
            
            boxRewardNoticeUIController.Init(milestoneReward);
        }
    }

    public void GrantMilestoneRewards(MilestoneReward milestone, bool isDouble = false)
    {
        foreach (var rewardData in milestone.Rewards)
        {
            int amount = rewardData.amount;
            if (isDouble)
            {
                amount *= 2;
            }
            GrantReward(rewardData.rewardType, amount);
        }
        Debug.Log($"마일스톤 달성: {milestone.StarRequirement}개의 별! 보상 지급 완료.");

        // 보상을 받았으므로 다시 받을 수 없는 상태로 설정
        milestone.IsClaimed = true;

        // 해당 마일스톤의 QuestRewardBox 상태 업데이트
        int index = milestoneRewards.IndexOf(milestone);
        if (index >= 0 && index < questRewardBoxes.Count)
        {
            questRewardBoxes[index].SetClaimed();
        }

        QuestManager.Instance.Save();
    }
    
    private void GrantReward(CurrencyType rewardType, int amount)
    {
        switch (rewardType)
        {
            case CurrencyType.Money:
                CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, amount * (int)(Mathf.Pow(1.5f, CollectionManager.Instance.currentWigCount) + 1e-6f));
                break;
            case CurrencyType.Gem:
                CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Gem, amount);
                break;
            case CurrencyType.AdRemovalTicket:
                //TODO : 실제 광고제거권 지급 로직 구현
                Debug.Log($"광고제거권 {amount} 지급");
                break;
            default:
                Debug.LogWarning("알 수 없는 보상 타입입니다.");
                break;
        }
    }
    
    private void SetBoxRewardNoticeUIPosition(QuestRewardBox questRewardBox, BoxRewardNoticeUIController boxRewardNoticeUIController)
    {
        RectTransform rewardBoxRect = questRewardBox.GetComponent<RectTransform>();
        RectTransform boxRewardNoticeRect = boxRewardNoticeUIController.GetComponent<RectTransform>();

        // 부모가 동일한지 확인
        if (rewardBoxRect.parent == boxRewardNoticeRect.parent)
        {
            // localPosition을 복사하여 위치 설정
            boxRewardNoticeRect.localPosition = rewardBoxRect.localPosition;
        }
        else
        {
            // 부모가 다른 경우, 위치 변환 필요
            Vector3 worldPosition = rewardBoxRect.position;
            Vector3 localPosition = boxRewardNoticeRect.parent.InverseTransformPoint(worldPosition);

            boxRewardNoticeRect.localPosition = localPosition;
        }
    }

    public bool CheckQuestSlotRedDot()
    {
        foreach (var qeust in questSlots)
        {
            if (qeust.Quest.IsCompleted)
            {
                return true;
            }
        }

        return false;
    }

    public void SetQuestUIInteractable(bool isInteractable)
    {
        foreach (var questRewardBox in questRewardBoxes)
        {
            questRewardBox.GetComponent<Button>().interactable = isInteractable;
        }

        foreach (var questSlot in questSlots)
        {
            questSlot.GetComponent<Button>().interactable = isInteractable;
        }
    }
}
