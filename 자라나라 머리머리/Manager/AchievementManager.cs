using UnityEngine;
using System.Collections.Generic;
using System;
using GameEvents;
using Sirenix.OdinInspector;

[Serializable]
public class AchievementSaveData
{
    public string groupID;
    public int order;
    public long currentPoints;
    public bool isRewardClaimed;

    public AchievementSaveData(string groupID, int order, long currentPoints, bool isComplete, bool isRewardClaimed)
    { 
        this.groupID = groupID;
        this.order = order;
        this.currentPoints = currentPoints;
        this.isRewardClaimed = isRewardClaimed;
    }
}

[Serializable]
public class AchievementCompletionRecord
{
    public string groupID;
    public int order;
    public string completeDate;
    
    public AchievementCompletionRecord(string groupID, int order, DateTime completionDate)
    {
        this.groupID = groupID;
        this.order = order;
        completeDate = completionDate.ToString("yy-MM-dd");
    }
}

[Serializable]
public class AchievementTitleSaveData
{
    public int level;
    public int currentPoints;
    
    public AchievementTitleSaveData(int level, int currentPoints)
    {
        this.level = level;
        this.currentPoints = currentPoints;
    }
}

public class AchievementManager : SingletonDontDestroy<AchievementManager>, ISavable
{
    [ShowInInspector]
    public Dictionary<string, List<Achievement>> achievementsDictionary = new();

    [ShowInInspector]
    public Dictionary<string, Achievement> currentAchievementSlots = new();
    
    [ShowInInspector]
    public Dictionary<int, AchievementTitle> achievementTitlesDictionary = new();
    
    public AchievementTitle currentAchievementTitle;

    public static int CurrentAchievementLevel
    {
        get
        {
            if (Instance.currentAchievementTitle != null)
            {
                return Instance.currentAchievementTitle.level;
            }
            else
            {
                Debug.LogWarning("currentAchievementTitle이 초기화되지 않았습니다.");
                return 0;
            }
        }
    }
    
    public RedDotComponent achivementRedDotComponent;
    
    protected override void Awake()
    {
        base.Awake();
        Register(this);
        LoadAchievementsFromCsv();
        LoadAchievementTitlesFromCsv();
    }

    private void Start()
    {
        // 오프라인 모드용
        if (SaveManager.Instance.saveData == null)
        {
            SetDefaultAchievement();
            SetDefaultAchievementTitle();
        }

        // 강화와 가발 현재 포인트 덮어쓰기
        OverwriteCurrentAchievementPoints();
        
        UIManager.Instance.GetUIView<AchievementZoneUIView>("AchievementZoneUI").Init(currentAchievementTitle.titleName, currentAchievementTitle.level);
        
        achivementRedDotComponent = UIManager.Instance.achievementIcon.GetComponentInChildren<RedDotComponent>();
        
        if (CheckAchievementRedDot())
        {
            achivementRedDotComponent.Register(RedDotChannel.Achievement);
        }
    }

    private void SubscribeEvent(string achievementType)
    {
        switch (achievementType)
        {
            case "로그인":
                EventBus.Subscribe<LoginEvent>(OnLogin);
                break;
            case "머리카락":
                EventBus.Subscribe<HairCutEvent>(OnHairCut);
                break;
            case "가발":
                EventBus.Subscribe<WigUnlockedEvent>(OnWigUnlocked);
                break;
            case "피버":
                EventBus.Subscribe<FeverActivatedEvent>(OnFeverActivated);
                break;
            case "광고":
                EventBus.Subscribe<AdWatchedEvent>(OnAdWatched);
                break;
            case "퀘스트":
                EventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
                break;
            case "강화":
                EventBus.Subscribe<EnhancementsEvent>(OnEnhancementCompleted);
                break;
            case "돈":
                EventBus.Subscribe<MoneyGetEvent>(OnMoenyGet);
                break;
        }
    }

    private void UnsubscribeEvent(string achievementType)
    {
        switch (achievementType)
        {
            case "로그인":
                EventBus.Unsubscribe<LoginEvent>(OnLogin);
                break;
            case "머리카락":
                EventBus.Unsubscribe<HairCutEvent>(OnHairCut);
                break;
            case "가발":
                EventBus.Unsubscribe<WigUnlockedEvent>(OnWigUnlocked);
                break;
            case "피버":
                EventBus.Unsubscribe<FeverActivatedEvent>(OnFeverActivated);
                break;
            case "광고":
                EventBus.Unsubscribe<AdWatchedEvent>(OnAdWatched);
                break;
            case "퀘스트":
                EventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
                break;
            case "강화":
                EventBus.Unsubscribe<EnhancementsEvent>(OnEnhancementCompleted);
                break;
            case "돈":
                EventBus.Unsubscribe<MoneyGetEvent>(OnMoenyGet);
                break;
        }
    }

    private void LoadAchievementsFromCsv()
    {
        TextAsset csvData = Resources.Load<TextAsset>("Achievements");
        if (csvData == null)
        {
            Debug.LogError("Achievements.csv not found in Resources folder.");
            return;
        }

        // CSV 파일 전체 라인을 파싱
        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 라인은 헤더이므로 스킵
        // 헤더 예시: "GroupID\tOrder\tDescription\tRewardPoints\tGoalValue"
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] columns = line.Split(',');

            // 컬럼 인덱스에 맞추어 데이터 파싱
            // GroupID	Order	Description	RewardPoints    GoalValue
            // 0        1       2           3               4
            if (columns.Length < 5)
            {
                Debug.LogWarning($"Invalid line format in CSV: {line}");
                continue;
            }

            string groupID = columns[0].Trim();

            if (!int.TryParse(columns[1].Trim(), out var order) ||
                !int.TryParse(columns[3].Trim(), out var rewardPoints) ||
                !int.TryParse(columns[4].Trim(), out var goalValue))
            {
                Debug.LogWarning($"Invalid numeric values in CSV line: {line}");
                continue;
            }

            string description = columns[2].Trim();

            Achievement achievement = new Achievement(groupID, order, description, goalValue, rewardPoints);

            // 딕셔너리에 해당 그룹ID 키가 없으면 새로 추가
            if (!achievementsDictionary.ContainsKey(groupID))
            {
                achievementsDictionary[groupID] = new List<Achievement>();
            }

            achievementsDictionary[groupID].Add(achievement);
        }

        // 각 그룹별로 Order 기준 정렬 (필요하다면)
        foreach (var kvp in achievementsDictionary)
        {
            kvp.Value.Sort((a, b) => a.order.CompareTo(b.order));
        }
    }
    
    private void LoadAchievementTitlesFromCsv()
    {
        TextAsset csvData = Resources.Load<TextAsset>("AchievementTitles");
        if (csvData == null)
        {
            Debug.LogError("AchievementTitles.csv not found in Resources folder.");
            return;
        }

        // CSV 파일 전체 라인을 파싱
        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 라인은 헤더이므로 스킵
        // 헤더 예시: "TitleName\tLevel\tRequiredPoints"
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] columns = line.Split(',');

            // 컬럼 인덱스에 맞추어 데이터 파싱
            // TitleName    Level   RequiredPoints
            // 0            1       2
            if (columns.Length < 3)
            {
                Debug.LogWarning($"Invalid line format in CSV: {line}");
                continue;
            }

            string titleName = columns[0].Trim();

            if (!int.TryParse(columns[1].Trim(), out var level) ||
                !int.TryParse(columns[2].Trim(), out var requiredPoints))
            {
                Debug.LogError($"Invalid numeric values in CSV line: {line}");
                continue;
            }

            AchievementTitle achievementTitle = new AchievementTitle(titleName, level, requiredPoints);

            // 딕셔너리에 추가
            achievementTitlesDictionary.TryAdd(level, achievementTitle);
        }
    }

    // 이벤트 핸들러들

    private void OnLogin(LoginEvent e)
    {
        UpdateAchievementProgress("로그인", 1);
    }

    private void OnHairCut(HairCutEvent e)
    {
        UpdateAchievementProgress("머리카락", 1);
    }

    private void OnAdWatched(AdWatchedEvent e)
    {
        UpdateAchievementProgress("광고", 1);
    }
    

    /*private void OnSpecialOctopusEncounter(SpecialOctopusEncounterEvent e)
    {
        UpdateAchievementProgress("특별한 문어", 1);
    }*/

    private void OnFeverActivated(FeverActivatedEvent e)
    {
        UpdateAchievementProgress("피버", 1);
    }
    
    private void OnMoenyGet(MoneyGetEvent e)
    {
        UpdateAchievementProgress("돈", e.Amount);
    }
    
    private void OnQuestCompleted(QuestCompletedEvent e)
    {
        UpdateAchievementProgress("퀘스트", 1);
    }
    
    private void OnEnhancementCompleted(EnhancementsEvent e)
    {
        UpdateAchievementProgress("강화", e.EnhancementCount);
    }

    private void OnWigUnlocked(WigUnlockedEvent e)
    {
        UpdateAchievementProgress("가발", e.WigCount);
    }
    
    private void UpdateAchievementProgress(string groupID, long amount)
    {
        foreach (var kvp in currentAchievementSlots)
        {
            // 누적이라 계속 currentPoints에 값 대입
            if (kvp.Key == groupID)
            {
                if (kvp.Key is "가발" or "강화")
                {
                    currentAchievementSlots[groupID].currentPoints = amount;
                }
                else
                {
                    currentAchievementSlots[groupID].AddCurrentPoints(amount);
                }

                // 업적 업데이트 이벤트 발행
                EventBus.Publish(new AchievementsUpdatedEvent(kvp.Value));

                if (kvp.Value.isCompleted)
                {
                    achivementRedDotComponent.Register(RedDotChannel.Achievement);

                    if (!CheckNextAchievement(groupID, kvp.Value.order + 1))
                    {
                        UnsubscribeEvent(groupID);
                    }
                }
            }
        }
    }
    
    public void Save()
    {
        var saveData = SaveManager.Instance.saveData;
        if (saveData == null)
            return;
        
        // 기존 목록 초기화
        saveData.achievementSaveDataList = new List<AchievementSaveData>();

        // currentAchievementSlots에 있는 업적 정보를 AchievementSaveData로 변환
        foreach (var kvp in currentAchievementSlots)
        {
            Achievement achievement = kvp.Value;

            // AchievementSaveData 생성
            var data = new AchievementSaveData(
                achievement.groupID,
                achievement.order,
                achievement.currentPoints,
                achievement.isCompleted,
                achievement.isRewardClaimed
            );

            saveData.achievementSaveDataList.Add(data);
        }

        saveData.achievementTitleSaveData = new AchievementTitleSaveData(currentAchievementTitle.level, currentAchievementTitle.currentPoints);
    }

    public void Load()
    {
        if (SaveManager.Instance.saveData != null)
        {
            // 기존 진행도 로드
            if (SaveManager.Instance.saveData.achievementSaveDataList != null)
            {
                LoadAchievementFromSaveData();
            }
            else
            {
                // achievementSaveDataList가 null인 경우
                SetDefaultAchievement();
            }

            if (SaveManager.Instance.saveData.achievementTitleSaveData != null)
            {
                LoadAchievementTitleFromSaveData();
            }
            else
            {
                SetDefaultAchievementTitle();
            }
        }
    }

    // 각 그룹마다 첫 번째 업적(가장 낮은 order)을 currentAchievementSlots에 넣기.
    private void SetDefaultAchievement()
    {
        currentAchievementSlots.Clear();
        foreach (var kvp in achievementsDictionary)
        {
            var groupID = kvp.Key;
            var achievementList = kvp.Value;
            if (achievementList.Count > 0)
            {
                // 정렬되어 있다고 가정하므로 첫 번째 요소가 가장 낮은 order 업적
                currentAchievementSlots[groupID] = achievementList[0];
                SubscribeEvent(groupID);
            }
        }
    }
    
    private void SetDefaultAchievementTitle()
    {
        currentAchievementTitle = null;

        // 1레벨 칭호 넣어주기
        currentAchievementTitle = achievementTitlesDictionary[1];
    }

    public void Register(ISavable savable)
    {
        SaveManager.Instance.RegisterSavable(this);
    }

    public void Unregister(ISavable savable)
    {
        SaveManager.Instance.UnregisterSavable(this);
    }

    private void LoadAchievementFromSaveData()
    {
        var saveData = SaveManager.Instance.saveData;
        if (saveData == null)
            return;
        
        if (saveData.achievementSaveDataList == null)
            return;
        
        // achievementSaveDataList가 비어있으면 기본 업적으로 세팅
        if (saveData.achievementSaveDataList.Count == 0)
        {
            SetDefaultAchievement();
            return;
        }
        
        // 각 그룹별로 최고 Order까지 완료 처리
        foreach (var data in saveData.achievementSaveDataList)
        {
            if (achievementsDictionary.TryGetValue(data.groupID, out var achievementList))
            {
                foreach (var achievement in achievementList)
                {
                    if (achievement.order < data.order)
                    {
                        achievement.isCompleted = true;
                        achievement.isRewardClaimed = true;
                    }
                    // currentAchievementSlots에 데이터 불러와서 넣기
                    else if (achievement.order == data.order)
                    {
                        currentAchievementSlots[data.groupID] = achievement;
                        currentAchievementSlots[data.groupID].currentPoints = data.currentPoints;
                        currentAchievementSlots[data.groupID].SetIsCompleted();
                        currentAchievementSlots[data.groupID].isRewardClaimed = data.isRewardClaimed; // 마지막 업적이 아닌이상 true로 들어오기 힘듬.
                        if (!CheckNextAchievement(data.groupID, data.order + 1) && currentAchievementSlots[data.groupID].isRewardClaimed)
                        {
                            UnsubscribeEvent(data.groupID);
                        }
                        else
                        {
                            SubscribeEvent(data.groupID);
                        }
                    }
                }
            }
            // GroupID가 없다면 
            else
            {
                Debug.LogWarning($"저장 데이터에 맞는 업적 '{data.groupID}'이, 업적 정보에 들어 있지 않습니다.");
            }
        }
        
        if (SaveManager.Instance.saveData.achievementCompletionRecords == null)
            return;
        // 이미 완료한 업적들 중에서 CompletionRecord에 기록된 날짜를 적용
        foreach (var record in saveData.achievementCompletionRecords)
        {
            if (achievementsDictionary.TryGetValue(record.groupID, out var achievementList))
            {
                var achievement = achievementList.Find(a => a.order == record.order);
                if (achievement != null && achievement.isCompleted)
                {
                    if (record.completeDate != null)
                    {
                        achievement.completeDate = record.completeDate;
                    }
                }
            }
        }
    }

    private void LoadAchievementTitleFromSaveData()
    {
        var saveData = SaveManager.Instance.saveData;
        if (saveData == null)
            return;

        if (saveData.achievementTitleSaveData == null)
        {
            SetDefaultAchievementTitle();
            return;
        }

        // 현재 저장된 칭호 레벨
        int savedLevel = saveData.achievementTitleSaveData.level;
        int savedCurrentPoints = saveData.achievementTitleSaveData.currentPoints;

        // 칭호 데이터에서 해당 레벨의 칭호를 가져오기
        if (achievementTitlesDictionary.TryGetValue(savedLevel, out var achievementTitle))
        {
            // 우선 현재 칭호로 설정
            currentAchievementTitle = achievementTitle;

            // 저장된 레벨보다 낮은 레벨의 칭호들에 대해 완료처리
            foreach (var kvp in achievementTitlesDictionary)
            {
                int level = kvp.Key;
                var title = kvp.Value;

                if (level < savedLevel)
                {
                    title.isCompleted = true;
                    title.isRewardClaimed = true;
                }
            }
            
            // 현재 레벨의 칭호 진행도 반영
            currentAchievementTitle.AddCurrentPoints(savedCurrentPoints);
        }
        else
        {
            // 저장된 레벨이지만 칭호 딕셔너리에 해당 레벨이 없다면?
            // 기본 칭호 설정
            SetDefaultAchievementTitle();
        }
    }

    // 다음 업적 정보를 매개변수에 넣어서 호출, 포인트 옮김 처리
    public void SetNextAchievement(string groupID, int nextOrder)
    {
        if (achievementsDictionary.TryGetValue(groupID, out var achievementList))
        {
            var achievement = achievementList.Find(a => a.order == nextOrder);
            if (achievement == null)
            {
                Debug.LogWarning($"{groupID}의 {nextOrder}번째의 업적이 존재 하지 않습니다.");
            }
            else
            {
                // 현재 업적 슬롯에서 해당 업적 갱신
                if (currentAchievementSlots[groupID].CheckIsCompleted())
                {
                    long remainPoints;
                    if (groupID == "가발")
                    {
                        remainPoints = CollectionManager.Instance.currentWigCount;
                    }
                    else if (groupID == "강화")
                    {
                        remainPoints = EnhancementManager.Instance.completedLine1Enhancements.Count + EnhancementManager.Instance.completedLine2Enhancements.Count;
                    }
                    else
                    {
                        remainPoints = currentAchievementSlots[groupID].currentPoints - currentAchievementSlots[groupID].goalValue;
                    }
                    currentAchievementSlots[groupID] = achievement;
                    currentAchievementSlots[groupID].currentPoints = remainPoints;
                }
                else
                {
                    Debug.LogError($"포인트가 충분하지 않은데 다음 업적으로 넘어갔습니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"{groupID}의 업적 리스트가 존재 하지 않습니다.");
        }
    }
    
    // 다음 칭호 레벨을 매개변수에 넣어서 호출, 포인트 옮김 처리
    public void SetNextAchievementTitle(int nextlevel)
    {
        if (CheckNextAchievementTitle(nextlevel))
        {
            if (currentAchievementTitle.CheckIsCompleted())
            {
                int remainPoints = currentAchievementTitle.currentPoints - currentAchievementTitle.requiredPoints;
                currentAchievementTitle = achievementTitlesDictionary[nextlevel];
                /*AchievementUIView achievementUIView = UIManager.Instance.GetUIView<AchievementUIView>("AchievementUI");
                if (achievementUIView != null)
                {
                    achievementUIView.UpdateTitle(currentAchievementTitle.titleName);
                    // 메인씬 칭호 업데이트
                    UIManager.Instance.GetUIView<AchievementZoneUIView>("AchievementZoneUI").Init(currentAchievementTitle.titleName, currentAchievementTitle.level);
                }
                else
                {
                    Debug.LogWarning("achievementUIView가 null입니다.");
                }*/
                currentAchievementTitle.AddCurrentPoints(remainPoints);
            }
        }
    }

    public bool CheckNextAchievement(string groupID, int nextOrder)
    {
        if (!achievementsDictionary.TryGetValue(groupID, out var achievementList))
        {
            Debug.LogError($"{groupID}의 업적 리스트가 존재하지 않습니다.");
            return false;
        }

        var achievement = achievementList.Find(a => a.order == nextOrder);
        if (achievement == null)
        {
            return false;
        }

        return true;
    }
    
    public bool CheckNextAchievementTitle(int nextLevel)
    {
        if (achievementTitlesDictionary.ContainsKey(nextLevel))
        {
            return true;
        }
        else
        {
            Debug.LogError($"'{nextLevel}' 레벨의 칭호가 존재하지 않습니다.");
            return false;
        }
    }

    public void SaveCompleteDate(string groupID, int order, Achievement achievement)
    {
        achievement.completeDate = TimeManager.Instance.GetCurrentServerTime().ToString("yy-MM-dd");

        if (SaveManager.Instance.saveData != null)
        {
            if (SaveManager.Instance.saveData.achievementCompletionRecords == null)
            {
                SaveManager.Instance.saveData.achievementCompletionRecords = new List<AchievementCompletionRecord>();
            }
            
            // 저장데이터에 같은 데이터가 있는지 확인
            var existingRecord = SaveManager.Instance.saveData.achievementCompletionRecords.Find(r => r.groupID == groupID && r.order == order);
            
            if (existingRecord == null)
            {
                // 완료 기록 추가
                SaveManager.Instance.saveData.achievementCompletionRecords.Add(
                    new AchievementCompletionRecord(groupID, order, TimeManager.Instance.GetCurrentServerTime())
                );
            }
        }
    }

    public bool CheckAchievementRedDot()
    {
        foreach (var kvp in currentAchievementSlots)
        {
            if (kvp.Value.CheckIsCompleted())
            {
                return true;
            }
        }
        
        return false;
    }

    // 가발과 강화 현재 갯수 덮어쓰기 용
    private void OverwriteCurrentAchievementPoints()
    {
        foreach (var kvp in currentAchievementSlots)
        {
            if (kvp.Key == "가발")
            {
                kvp.Value.currentPoints = CollectionManager.Instance.currentWigCount;
            }
            else if (kvp.Key == "강화")
            {
                kvp.Value.currentPoints = EnhancementManager.Instance.completedLine1Enhancements.Count 
                                          + EnhancementManager.Instance.completedLine2Enhancements.Count;
            }
        }
    }

    public void EnhanceEventPublish()
    {
        EventBus.Publish(new EnhancementsEvent());
    }
}
