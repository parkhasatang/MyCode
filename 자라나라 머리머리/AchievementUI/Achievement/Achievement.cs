using System;

[Serializable]
public class Achievement
{
    public string groupID;
    public int order;
    public long currentPoints;
    public string description;
    public int goalValue;
    public int rewardPoints;
    public bool isCompleted;
    public bool isRewardClaimed;
    public string completeDate;

    public Achievement(string groupID, int order, string description, int goalValue, int rewardPoints)
    {
        this.groupID = groupID;
        this.order = order;
        this.description = description;
        this.goalValue = goalValue;
        this.rewardPoints = rewardPoints;
        isCompleted = false;
        isRewardClaimed = false;
        completeDate = "";
    }
    
    public void AddCurrentPoints(long amount)
    {
        currentPoints += amount;
    }

    public bool CheckIsCompleted()
    {
        return currentPoints >= goalValue;
    }

    public void SetIsCompleted()
    {
        isCompleted = CheckIsCompleted();
    }
}

[Serializable]
public class AchievementTitle
{
    public string titleName;
    public int requiredPoints;
    public int level;
    public int currentPoints;
    public bool isCompleted;
    public bool isRewardClaimed; // 나중에 보상을 줄 수 있을지도 몰라 추가
    
    public AchievementTitle(string titleName, int level, int requiredPoints)
    {
        this.titleName = titleName;
        this.level = level;
        this.requiredPoints = requiredPoints;
    }
    
    public void AddCurrentPoints(int amount)
    {
        currentPoints += amount;
    }
    
    public bool CheckIsCompleted()
    {
        return currentPoints >= requiredPoints;
    }
}