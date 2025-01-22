using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxRewardNoticeUIController : UIController
{
    public List<RewardNotice> rewardNotices = new();
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(MilestoneReward rewards)
    {
        foreach (var rewardNotice in rewardNotices)
        {
            rewardNotice.gameObject.SetActive(false);
        }
        for (int i = 0; i < rewards.Rewards.Count; i++)
        {
            rewardNotices[i].gameObject.SetActive(true);
            rewardNotices[i].iconImage.sprite = rewards.Rewards[i].rewardIcon;
            if (rewards.Rewards[i].rewardType == CurrencyType.Money)
            {
                int newAmount = rewards.Rewards[i].amount * (int)(Mathf.Pow(1.5f, CollectionManager.Instance.currentWigCount) + 1e-6f);
                rewardNotices[i].amountText.text = CurrencySystem.ToCurrencyString(newAmount);
            }
            else
            {
                rewardNotices[i].amountText.text = rewards.Rewards[i].amount.ToString();
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rewardNotices[i].rectTransform);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
