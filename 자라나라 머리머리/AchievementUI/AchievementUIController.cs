using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUIController : UIController
{
    [HideInInspector]
    public AchievementUIView achievementUIView;
    public Material starMaterial;
    
    private List<AchievementSlot> achievementSlots = new();

    private void Awake()
    {
        achievementUIView = GetComponent<AchievementUIView>();
    }

    private void Start()
    {
        // 게임 시작 후에 데이터를 추가할 때
        CurrencyEffectData newCurrencyData = new CurrencyEffectData
        {
            currencyType = CurrencyType.AchievementStar,
            currencyMaterial = starMaterial,
            currencyTransform = achievementUIView.starPosition
        };

        GameDataManager.Instance.AddCurrencyEffectData(newCurrencyData);

        GenerateAchievementSlots();
        achievementUIView.UpdateCurrentStarCountText();
        UpdateSlotOrder();
    }
    
    private void GenerateAchievementSlots()
    {
        var achievements = AchievementManager.Instance.currentAchievementSlots;

        foreach (var achievement in achievements)
        {
            CreateAchievementSlot(achievement.Value);
        }
        
        achievementUIView.achievementSlotContentTransform.localPosition = Vector3.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(achievementUIView.achievementSlotContentTransform);
    }
    
    private void CreateAchievementSlot(Achievement achievement)
    {
        GameObject achievementSlotPrefab = ResourceManager.Instance.GetResource<GameObject>("AchievementSlot");
        GameObject achievementSlotInstance = Instantiate(achievementSlotPrefab, achievementUIView.achievementSlotContentTransform);
        AchievementSlot achievementSlot = achievementSlotInstance.GetComponent<AchievementSlot>();
        achievementSlot.Initialize(achievement);

        // 리스트에 요소 추가
        achievementSlots.Add(achievementSlot);
    }
    
    public void UpdateSlotOrder()
    {
        // 완료 가능한 슬롯과 완료된 슬롯을 분류
        List<AchievementSlot> completableSlots = new();
        List<AchievementSlot> completedSlots = new();
        List<AchievementSlot> inProgressSlots = new();

        foreach (var slot in achievementSlots)
        {
            if (completableSlots.Contains(slot) || completedSlots.Contains(slot) || inProgressSlots.Contains(slot))
            {
                // 슬롯이 이미 리스트 중 하나에 포함되어 있으면 건너뜀
                continue;
            }

            if (slot.Achievement.isCompleted && !slot.Achievement.isRewardClaimed)
            {
                completableSlots.Add(slot);
            }
            else if (slot.Achievement.isRewardClaimed)
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
    }
}