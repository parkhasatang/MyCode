using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppableUI : MonoBehaviour, IDropHandler
{
    private int inventoryIndex;

    private void Start()
    {
        inventoryIndex = GetComponent<DraggableUI>().inventoryIndex;
    }

    // 기존에 있던 위치의 UI를 지금 있는 것과 바꿔주어야댐
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<Image>().sprite != null)
        {
            // 데이터 임시저장소에 올리기.
            UIManager.Instance.takeTemporaryItemData = UIManager.Instance.playerInventoryData.slots[inventoryIndex].item;
            UIManager.Instance.takeTemporaryItemStack = UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].item = null;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack = 0;
            
            // 데이터 받아오기.
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].item = UIManager.Instance.giveTemporaryItemData;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack = UIManager.Instance.giveTemporaryItemStack;
            UIManager.Instance.giveTemporaryItemData = null;
            UIManager.Instance.giveTemporaryItemStack = 0;
            EquipManager.Instance.UpdatePlayerStat();
            UIManager.Instance.UpdatePlayerStatTxt();
        }
        // 이미지 처리
        UIManager.Instance.playerInventoryData.StackUpdate(inventoryIndex);
    }
}
