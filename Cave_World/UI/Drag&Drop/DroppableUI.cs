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

    // ������ �ִ� ��ġ�� UI�� ���� �ִ� �Ͱ� �ٲ��־�ߴ�
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<Image>().sprite != null)
        {
            // ������ �ӽ�����ҿ� �ø���.
            UIManager.Instance.takeTemporaryItemData = UIManager.Instance.playerInventoryData.slots[inventoryIndex].item;
            UIManager.Instance.takeTemporaryItemStack = UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].item = null;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack = 0;
            
            // ������ �޾ƿ���.
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].item = UIManager.Instance.giveTemporaryItemData;
            UIManager.Instance.playerInventoryData.slots[inventoryIndex].stack = UIManager.Instance.giveTemporaryItemStack;
            UIManager.Instance.giveTemporaryItemData = null;
            UIManager.Instance.giveTemporaryItemStack = 0;
            EquipManager.Instance.UpdatePlayerStat();
            UIManager.Instance.UpdatePlayerStatTxt();
        }
        // �̹��� ó��
        UIManager.Instance.playerInventoryData.StackUpdate(inventoryIndex);
    }
}
