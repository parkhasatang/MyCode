using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Login = 0,
    WatchAds = 1,
    CollectHair = 2,
    ActivateFever = 3,
    OpenCollection = 4
}

[Serializable]
public class QuestData
{
    public int questId;
    public string questName;
    public QuestType questType;
    public int targetAmount;
    public int starReward; // 별 보상에 대한 정보
}

[Serializable]
public class RewardData
{
    public CurrencyType rewardType;
    public int amount;
    public string iconPath; // 아이콘 경로

    [NonSerialized]
    public Sprite rewardIcon; // 실제 이미지 로드된 아이콘
}

[Serializable]
public class MilestoneReward
{
    public int StarRequirement;
    public List<RewardData> Rewards;
    
    public bool IsClaimed; // 보상을 받았는지 여부
}