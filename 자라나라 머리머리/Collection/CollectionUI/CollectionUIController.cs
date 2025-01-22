using System;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUIController : UIController
{
    public GameObject hairStyleContent;
    private Dictionary<string, WigSlot> wigSlotDict = new Dictionary<string, WigSlot>();

    private void Awake()
    {
        // 가발 컬렉션 설정
        SetWigCollection();

        // 컬렉션 열림 이벤트 발행
        EventBus.Publish(new CollectionOpenedEvent());
        
        // 이벤트 구독
        CollectionManager.Instance.OnWigUnlocked += OnWigUnlocked;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.OnWigUnlocked -= OnWigUnlocked;
        }
    }

    private void SetWigCollection()
    {
        // 가발 데이터 리스트를 통해 UI 슬롯 생성
        foreach (var wigData in CollectionManager.Instance.wigDataList)
        {
            GameObject wigSlotPrefab = ResourceManager.Instance.GetResource<GameObject>("WigSlot");
            GameObject wigSlotObj = Instantiate(wigSlotPrefab, hairStyleContent.transform);
            WigSlot wigSlotComponent = wigSlotObj.GetComponent<WigSlot>();

            // 슬롯 초기화
            wigSlotComponent.Initialize(wigData);
            wigSlotDict.Add(wigData.UniqueID, wigSlotComponent);
        }
        
        hairStyleContent.transform.position = Vector3.zero;
    }

    private void OnWigUnlocked(string uniqueID)
    {
        // 해당 가발 슬롯의 UI 업데이트
        if (wigSlotDict.TryGetValue(uniqueID, out var wigSlot))
        {
            wigSlot.UpdateUI();
        }
    }
}