using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Sirenix.OdinInspector;

[Serializable]
public class WigData
{
    public WigSO wigSO; // 가발 스크립터블 오브젝트
    public bool isUnlocked; // 가발 잠금 해제 여부
    public bool isFirstChecked; // 첫 확인 여부

    public string UniqueID => wigSO.uniqueID; // 고유 ID 반환

    public WigData(WigSO wigSO, bool isUnlocked = false, bool isFirstChecked = false)
    {
        this.wigSO = wigSO;
        this.isUnlocked = isUnlocked;
        this.isFirstChecked = isFirstChecked;
    }
}

[Serializable]
public class CollectionSaveData
{
    public string uniqueID; // 가발 ID
    public bool isFirstChecked; // 첫 확인 여부

    public CollectionSaveData(string uniqueID, bool isFirstChecked)
    {
        this.uniqueID = uniqueID;
        this.isFirstChecked = isFirstChecked;
    }
}

public class CollectionManager : Singleton<CollectionManager>, ISavable
{
    public List<WigData> wigDataList = new(); // 가발 데이터 리스트
    [ShowInInspector]
    public int currentWigCount => wigDataList.Count(x => x.isUnlocked); // 현재 가발 해금 수

    [ShowInInspector]
    private Dictionary<string, WigData> wigDataDictionary = new();
    // 가발 체크를 위한 카운트
    private int unCheckedUnlockedWigCount = 0;

    public WigSO currentWigInfoData; // 현재 가발 정보 데이터
    
    public RedDotComponent collectionRedDotComponent;
    
    // 가발 잠금 해제 시 발생하는 이벤트
    public event Action<string> OnWigUnlocked;
    
    // 팝업 중복 해결을 위한 Queue
    private Queue<WigSO> newWigQueue = new();
    private bool isNewWigUIShowing = false;
    private bool isScheduleShowing = false; // 코루틴 중복 실행 방지용

    
    protected override void Awake()
    {
        collectionRedDotComponent = UIManager.Instance.galleryIcon.GetComponentInChildren<RedDotComponent>();
        base.Awake();
        Register(this);
    }
    
    public void LoadWigs()
    {
        wigDataList.Clear(); // 기존 리스트를 초기화합니다.
        wigDataDictionary.Clear(); // 딕셔너리도 초기화합니다.

        var uniqueWigSOs = new HashSet<WigSO>(); // 중복 제거를 위한 HashSet

        foreach (var hairInfo in GameDataManager.Instance.HairDatas)
        {
            var wigSO = hairInfo.WigSO;

            if (uniqueWigSOs.Add(wigSO)) // HashSet에 추가되면 true 반환
            {
                var wigData = new WigData(wigSO);
                wigDataList.Add(wigData);
                wigDataDictionary.Add(wigSO.uniqueID, wigData);
            }
        }

        foreach (var hairInfo in GameDataManager.Instance.eventHairs)
        {
            var wigSO = hairInfo.WigSO;

            if (uniqueWigSOs.Add(wigSO)) // HashSet에 추가되면 true 반환
            {
                var wigData = new WigData(wigSO);
                wigDataList.Add(wigData);
                wigDataDictionary.Add(wigSO.uniqueID, wigData);
            }
        }
        
        // wigDataList를 wigSO의 index를 기준으로 정렬
        wigDataList = wigDataList.OrderBy(w => w.wigSO.index).ToList();
    }

    void LoadUnlockData()
    {
        if (SaveManager.Instance.saveData.unlockedWigs != null)
        {
            List<CollectionSaveData> savedDataList = SaveManager.Instance.saveData.unlockedWigs;

            unCheckedUnlockedWigCount = 0;
            foreach (var savedData in savedDataList)
            {
                var wigData = GetWigDataByID(savedData.uniqueID);
                if (wigData != null)
                {
                    wigData.isUnlocked = true;
                    wigData.isFirstChecked = savedData.isFirstChecked;

                    if (!wigData.isFirstChecked)
                    {
                        unCheckedUnlockedWigCount++;
                    }
                }
                else
                {
                    Debug.LogWarning($"로드 중 {savedData.uniqueID}에 해당하는 WigData를 찾을 수 없습니다.");
                }
            }

            if (HasUnseenUnlockedWigs())
            {
                collectionRedDotComponent.Register(RedDotChannel.Gallery);
            }
        }
        else
        {
            Debug.Log("저장된 해금된 가발 데이터가 없습니다.");
        }
    }

    public void UnlockWig(string uniqueID)
    {
        var wigData = GetWigDataByID(uniqueID);
        if (wigData != null && !wigData.isUnlocked) 
        {
            wigData.isUnlocked = true;
            unCheckedUnlockedWigCount++;
            EventBus.Publish(new WigUnlockedEvent());

            if (SaveManager.Instance.saveData != null)
            {
                Save();
            }
            else
            {
                SaveManager.Instance.SaveAll();
            }
            
            // UI 큐에 추가
            newWigQueue.Enqueue(wigData.wigSO);

            // 현재 UI가 표시중이 아니고, 아직 코루틴으로 예약을 안 했다면 예약 시작
            if(!isNewWigUIShowing && !isScheduleShowing)
            {
                StartCoroutine(ScheduleShowNextWigUI());
            }

            OnWigUnlocked?.Invoke(uniqueID);
            collectionRedDotComponent.Register(RedDotChannel.Gallery); // 도감 오브젝트가 프리팹이라 여기서 레지스트 해줘야함
        }
    }

    public void SetWigFirstChecked(string uniqueID)
    {
        var wigData = GetWigDataByID(uniqueID);
        if (wigData == null) return;

        if (!wigData.isFirstChecked && wigData.isUnlocked && HasUnseenUnlockedWigs())
        {
            wigData.isFirstChecked = true;
            unCheckedUnlockedWigCount--;
            CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Gem, 5); // 보상 지급

            Save();
        }
    }

    public bool IsWigUnlocked(string uniqueID)
    {
        var wigData = GetWigDataByID(uniqueID);
        return wigData != null && wigData.isUnlocked;
    }

    private WigData GetWigDataByID(string uniqueID)
    {
        if (wigDataDictionary.TryGetValue(uniqueID, out var wigData))
        {
            return wigData;
        }
        else
        {
            return null;
        }
    }

    private void ShowNewWigNotice(WigSO wigSO)
    {
        UIManager.Instance.ShowUI("NewWigNoticeUI");

        var newWigNoticeUIController = UIManager.Instance.GetUIController<NewWigNoticeUIController>("NewWigNoticeUI");
        if (newWigNoticeUIController != null)
        {
            newWigNoticeUIController.SetupNewWigNoticeUI(wigSO); // UI 설정
        }
        else
        {
            Debug.LogError("NewWigNoticeUIController를 찾을 수 없습니다.");
        }
    }
    
    private void ShowNextWigUI()
    {
        if(newWigQueue.Count == 0)
        {
            GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true);
            return;
        }
        
        GameManager.Instance.Player.PluckController.SetPointerInputEnabled(false);
        // 큐에서 하나 꺼내서 UI 표시
        var wigSO = newWigQueue.Dequeue();

        Time.timeScale = 0f;
        isNewWigUIShowing = true;
        ShowNewWigNotice(wigSO);
    }
    
    public void OnNewWigUIClose()
    {
        Time.timeScale = 1f;
        isNewWigUIShowing = false;
        GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true);
        // 다음 UI가 대기중이라면 다시 표시
        if (newWigQueue.Count > 0)
        {
            ShowNextWigUI();
        }
    }
    
    private IEnumerator ScheduleShowNextWigUI()
    {
        isScheduleShowing = true;
        // 다음 프레임까지 대기 -> 한 프레임 안에 발생한 여러 UnlockWig 이벤트를 모두 큐에 쌓은 후 처리
        yield return null;

        ShowNextWigUI();
        isScheduleShowing = false;
    }
    
    public bool HasUnseenUnlockedWigs()
    {
        return unCheckedUnlockedWigCount > 0;
    }

    public void Save()
    {
        if (SaveManager.Instance.saveData != null)
        {
            var saveDataList = new List<CollectionSaveData>();
            foreach (var wigData in wigDataList)
            {
                if (wigData.isUnlocked)
                {
                    saveDataList.Add(new CollectionSaveData(wigData.UniqueID, wigData.isFirstChecked));
                }
            }

            SaveManager.Instance.saveData.unlockedWigs = saveDataList;
        }
    }

    public void Load()
    {
        LoadUnlockData(); // 해금 데이터 로드
    }

    public void Register(ISavable savable)
    {
        SaveManager.Instance.RegisterSavable(savable);
    }

    public void Unregister(ISavable savable)
    {
        SaveManager.Instance.UnregisterSavable(savable);
    }

    private void OnDestroy()
    {
        Unregister(this);
    }
}
