using System;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Sirenix.OdinInspector;

[Serializable]
public class QuestConfig
{
    public List<QuestData> quests;
}

[Serializable]
public class RewardConfig
{
    public List<MilestoneReward> milestoneRewards;
}

[Serializable]
public class QuestSaveData
{
    public int questId;
    public int currentAmount;
    public bool isRewardClaimed;

    public QuestSaveData(int questId, int currentAmount, bool isRewardClaimed)
    {
        this.questId = questId;
        this.currentAmount = currentAmount;
        this.isRewardClaimed = isRewardClaimed;
    }
}

[Serializable]
public class MilestoneRewardSaveData
{
    public int starRequirement;
    public bool isClaimed;

    public MilestoneRewardSaveData(int starRequirement, bool isClaimed)
    {
        this.starRequirement = starRequirement;
        this.isClaimed = isClaimed;
    }
}

public class QuestManager : SingletonDontDestroy<QuestManager>, ISavable
{
    // 퀘스트 정보
    [ShowInInspector]
    private List<Quest> quests = new();
    public List<Quest> Quests => quests;
    
    public int currentStarCount;

    // 퀘스트 보상
    [ShowInInspector]
    private List<MilestoneReward> milestoneRewards = new();
    public List<MilestoneReward> MilestoneRewards => milestoneRewards;
    
    internal RedDotComponent questRedDotComponent;
    
    protected override void Awake()
    {
        base.Awake();
        Register(this);
        // 퀘스트 로드
        LoadQuestsFromJson();
        // 보상정보 로드
        LoadMilestoneRewardsFromJson();
    }

    private void Start()
    {
        if (SaveManager.Instance.saveData == null || SaveManager.Instance.saveData.questSaveDataList == null)
        {
            foreach (var quest in quests)
            {
                if (!quest.IsCompleted)
                {
                    SubscribeEvent(quest.data.questType);
                }
            }
        }
        
        questRedDotComponent = UIManager.Instance.questIcon.GetComponentInChildren<RedDotComponent>();
        
        // 퀘스트 정보 다 불러오고 보상 받을 것이 있다면 레드닷 표시
        if (CheckQuestRedDot())
        {
            questRedDotComponent.Register(RedDotChannel.Quest);
        }
    }

    private void SubscribeEvent(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Login:
                EventBus.Subscribe<LoginEvent>(OnLogin);
                break;
            case QuestType.CollectHair:
                EventBus.Subscribe<HairCutEvent>(OnHairCut);
                break;
            case QuestType.WatchAds:
                EventBus.Subscribe<AdWatchedEvent>(OnAdWatched);
                break;
            case QuestType.ActivateFever:
                EventBus.Subscribe<FeverActivatedEvent>(OnFeverActivated);
                break;
            case QuestType.OpenCollection:
                EventBus.Subscribe<CollectionOpenedEvent>(OnCollectionOpened);
                break;
        }
    }

    public void UnsubscribeEvent(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Login:
                EventBus.Unsubscribe<LoginEvent>(OnLogin);
                break;
            case QuestType.CollectHair:
                EventBus.Unsubscribe<HairCutEvent>(OnHairCut);
                break;
            case QuestType.WatchAds:
                EventBus.Unsubscribe<AdWatchedEvent>(OnAdWatched);
                break;
            case QuestType.ActivateFever:
                EventBus.Unsubscribe<FeverActivatedEvent>(OnFeverActivated);
                break;
            case QuestType.OpenCollection:
                EventBus.Unsubscribe<CollectionOpenedEvent>(OnCollectionOpened);
                break;
        }
    }

    private void LoadQuestsFromJson()
    {
        // Resources 폴더에서 Quests.json 파일 로드
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Load/Quests");

        if (jsonTextAsset != null)
        {
            QuestConfig questConfig = JsonUtility.FromJson<QuestConfig>(jsonTextAsset.text);

            foreach (var questData in questConfig.quests)
            {
                quests.Add(new Quest(questData));
            }

            Debug.Log($"퀘스트 {quests.Count}개 로드 완료.");
        }
        else
        {
            Debug.LogError("Quests.json 파일을 Resources 폴더에서 찾을 수 없습니다.");
        }
    }
    
    private void LoadMilestoneRewardsFromJson()
    {
        // Resources 폴더에서 Rewards.json 파일 로드
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Load/Rewards");

        if (jsonTextAsset != null)
        {
            RewardConfig rewardConfig = JsonUtility.FromJson<RewardConfig>(jsonTextAsset.text);

            milestoneRewards = rewardConfig.milestoneRewards;

            // 아이콘 로드
            foreach (var milestone in milestoneRewards)
            {
                foreach (var reward in milestone.Rewards)
                {
                    reward.rewardIcon = ResourceManager.Instance.GetResource<Sprite>($"{reward.rewardType.ToString()}");
                    if (reward.rewardIcon == null)
                    {
                        Debug.LogWarning($"아이콘을 로드할 수 없습니다: {reward.iconPath}");
                    }
                }
            }

            Debug.Log($"마일스톤 보상 {milestoneRewards.Count}개 로드 완료.");
        }
        else
        {
            Debug.LogError("Rewards.json 파일을 Resources 폴더에서 찾을 수 없습니다.");
        }
    }

    // 이벤트 핸들러들

    private void OnLogin(LoginEvent e)
    {
        UpdateQuestProgress(QuestType.Login, 1);
    }

    private void OnHairCut(HairCutEvent e)
    {
        UpdateQuestProgress(QuestType.CollectHair, e.Amount);
    }

    private void OnAdWatched(AdWatchedEvent e)
    {
        UpdateQuestProgress(QuestType.WatchAds, e.Amount);
    }

    private void OnFeverActivated(FeverActivatedEvent e)
    {
        UpdateQuestProgress(QuestType.ActivateFever, e.Amount);
    }

    private void OnCollectionOpened(CollectionOpenedEvent e)
    {
        UpdateQuestProgress(QuestType.OpenCollection, 1);
    }

    private void UpdateQuestProgress(QuestType questType, int amount)
    {
        foreach (var quest in quests)
        {
            if (quest.data.questType == questType && !quest.IsCompleted)
            {
                quest.UpdateProgress(amount);

                // 퀘스트 업데이트 이벤트 발행
                EventBus.Publish(new QuestUpdatedEvent(quest));

                if (quest.IsCompleted)
                {
                    // 퀘스트 완료 이벤트 발행
                    questRedDotComponent.Register(RedDotChannel.Quest);
                    Debug.Log($"퀘스트 '{quest.data.questName}' 완료!");
                    
                    // 퀘스트가 완료되었으므로 이벤트 구독 해제
                    UnsubscribeEvent(questType);
                }
            }
        }
    }

    public bool CheckQuestRedDot()
    {
        foreach (var milestoneReward in milestoneRewards)
        {
            if (currentStarCount >= milestoneReward.StarRequirement && !milestoneReward.IsClaimed)
            {
                return true;
            }
        }
        
        foreach (var quest in quests)
        {
            if (quest.IsCompleted && !quest.isRewardClaimed)
            {
                return true;
            }
        }

        return false;
    }
    
    private List<QuestSaveData> ConvertQuestsToSaveData()
    {
        List<QuestSaveData> questSaveDataList = new List<QuestSaveData>();
        foreach (var quest in quests)
        {
            questSaveDataList.Add(new QuestSaveData(quest.data.questId, quest.currentAmount, quest.isRewardClaimed));
        }
        return questSaveDataList;
    }

    private void LoadQuestsFromSaveData(List<QuestSaveData> questSaveDataList)
    {
        // QuestSaveData를 questId를 키로 하는 딕셔너리로 맵핑
        Dictionary<int, QuestSaveData> questSaveDataDict = new Dictionary<int, QuestSaveData>();
        foreach (var saveData in questSaveDataList)
        {
            questSaveDataDict[saveData.questId] = saveData;
        }

        // 각 Quest에 대해 저장된 진행 상황을 적용
        foreach (var quest in quests)
        {
            if (questSaveDataDict.TryGetValue(quest.data.questId, out QuestSaveData saveData))
            {
                quest.currentAmount = saveData.currentAmount;
                quest.isRewardClaimed = saveData.isRewardClaimed;
            }
            else
            {
                // 저장된 데이터가 없으면 기본값으로 유지하거나 필요한 경우 초기화
                quest.currentAmount = 0;
                quest.isRewardClaimed = false;
            }
            
            // 퀘스트가 완료되지 않았으면 이벤트 구독
            if (!quest.IsCompleted)
            {
                SubscribeEvent(quest.data.questType);
            }
        }
    }

    private List<MilestoneRewardSaveData> ConvertMilestoneRewardsToSaveData()
    {
        List<MilestoneRewardSaveData> rewardSaveDataList = new List<MilestoneRewardSaveData>();
        foreach (var reward in milestoneRewards)
        {
            rewardSaveDataList.Add(new MilestoneRewardSaveData(reward.StarRequirement, reward.IsClaimed));
        }
        return rewardSaveDataList;
    }

    private void LoadMilestoneRewardsFromSaveData(List<MilestoneRewardSaveData> rewardSaveDataList)
    {
        // 기존 milestoneRewards 리스트를 초기화
        milestoneRewards.Clear();

        // JSON에서 마일스톤 보상을 다시 로드
        LoadMilestoneRewardsFromJson();

        // StarRequirement를 키로 하는 딕셔너리를 생성
        Dictionary<int, MilestoneReward> rewardDictionary = new Dictionary<int, MilestoneReward>();
        foreach (var reward in milestoneRewards)
        {
            rewardDictionary.Add(reward.StarRequirement, reward);
        }

        // 저장된 진행 상황을 기반으로 마일스톤 보상을 복원
        foreach (var saveData in rewardSaveDataList)
        {
            if (rewardDictionary.TryGetValue(saveData.starRequirement, out MilestoneReward reward))
            {
                reward.IsClaimed = saveData.isClaimed;
            }
            else
            {
                Debug.LogWarning($"StarRequirement {saveData.starRequirement}에 해당하는 MilestoneReward를 찾을 수 없습니다.");
            }
        }
    }

    public void Save()
    {
        if (SaveManager.Instance.saveData != null)
        {
            SaveManager.Instance.saveData.starCount = currentStarCount;
            SaveManager.Instance.saveData.milestoneRewards = ConvertMilestoneRewardsToSaveData();
            SaveManager.Instance.saveData.questSaveDataList = ConvertQuestsToSaveData();

            // 현재 서버 시간을 lastLogoutTime에 저장 (한국 시간)
            var serverTimeKorea = TimeManager.Instance.GetCurrentServerTime();
            SaveManager.Instance.saveData.lastLogoutTime = serverTimeKorea.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public void Load()
    {
        if (SaveManager.Instance.saveData != null)
        {
            var serverTimeKorea = TimeManager.Instance.GetCurrentServerTime(); // 이미 한국 시간

            DateTime lastLogoutKorea;
            
            // 저장된 lastLogoutTime이 있는지 확인
            if (!string.IsNullOrEmpty(SaveManager.Instance.saveData.lastLogoutTime))
            {
                if (DateTime.TryParse(SaveManager.Instance.saveData.lastLogoutTime, out var lastLogoutTime))
                {
                    lastLogoutKorea = lastLogoutTime; // 이미 한국 시간으로 저장되었다고 가정
                }
                else
                {
                    Debug.LogError("lastLogoutTime을 파싱하는 데 실패했습니다.");
                    // 파싱 실패 시 현재 서버 시간으로 설정
                    lastLogoutKorea = serverTimeKorea;
                }
            }
            else
            {
                // 저장된 lastLogoutTime이 없으면 현재 서버 시간으로 설정
                lastLogoutKorea = serverTimeKorea;
            }

            // 자정이 지났는지 확인
            if (serverTimeKorea.Date > lastLogoutKorea.Date)
            {
                // 자정이 지났으므로 퀘스트 진행도를 로드하지 않고 초기화
                currentStarCount = 0;
                Debug.Log("자정이 지나 퀘스트를 초기화합니다.");

                // 퀘스트 초기화
                quests.Clear();
                LoadQuestsFromJson();

                // 이벤트 구독 관리
                foreach (var quest in quests)
                {
                    if (!quest.IsCompleted)
                    {
                        SubscribeEvent(quest.data.questType);
                    }
                }

                // 초기화된 퀘스트 데이터를 저장
                Save();
            }
            else
            {
                // 기존 진행도 로드
                if (SaveManager.Instance.saveData.questSaveDataList != null)
                {
                    currentStarCount = SaveManager.Instance.saveData.starCount;
                    LoadMilestoneRewardsFromSaveData(SaveManager.Instance.saveData.milestoneRewards);
                    LoadQuestsFromSaveData(SaveManager.Instance.saveData.questSaveDataList);
                }
            }
        }
        else
        {
            foreach (var quest in quests)
            {
                if (!quest.IsCompleted)
                {
                    SubscribeEvent(quest.data.questType);
                }
            }
        }
    }

    public void Register(ISavable savable)
    {
        SaveManager.Instance.RegisterSavable(this);
    }

    public void Unregister(ISavable savable)
    {
        SaveManager.Instance.UnregisterSavable(this);
    }

    private void OnDestroy()
    {
        Unregister(this);
    }
}

