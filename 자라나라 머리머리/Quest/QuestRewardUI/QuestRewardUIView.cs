    using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardUIView : UIView
{
    public List<GameObject> rewardItems;

    public void UpdateRewardUI(MilestoneReward milestoneReward)
    {
        // 모든 보상 아이템 UI를 비활성화
        foreach (var rewardItem in rewardItems)
        {
            rewardItem.gameObject.SetActive(false);
        }

        // 현재 보상 리스트 UI에 활성화하고 설정
        for (int i = 0; i < milestoneReward.Rewards.Count && i < rewardItems.Count; i++)
        {
            var rewardData = milestoneReward.Rewards[i];
            var rewardItemUI = rewardItems[i];

            // 보상 아이템 UI 활성화
            rewardItemUI.gameObject.SetActive(true);

            // 보상 아이콘과 수량 설정
            rewardItemUI.GetComponentInChildren<Image>().sprite = rewardData.rewardIcon;
            if (rewardData.rewardType == CurrencyType.Money)
            {
                int newAmount = rewardData.amount * (int)(Mathf.Pow(1.5f, CollectionManager.Instance.currentWigCount) + 1e-6f);
                rewardItemUI.GetComponentInChildren<TMP_Text>().text = CurrencySystem.ToCurrencyString(newAmount);
            }
            else
            {
                rewardItemUI.GetComponentInChildren<TMP_Text>().text = rewardData.amount.ToString();
            }
        }
    }
}