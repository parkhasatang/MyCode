using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 자기가 몇번 째 인벤토리인지 알고싶음.
    // 나중에 Awake로 번호 찾아오기 구현.
    [SerializeField] internal int inventoryIndex;

    private Transform canvas;
    private Transform previousParent;
    private RectTransform uiItemTransform;
    private CanvasGroup itemImg;
    private Inventory inventory;

    private Item previousItem;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        uiItemTransform = GetComponent<RectTransform>();
        itemImg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        inventory = UIManager.Instance.playerInventoryData;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;

        if (!inventory.slots[inventoryIndex].isEmpty)
        {
            transform.SetParent(canvas);
            transform.SetAsLastSibling();
            previousItem = UIManager.Instance.playerInventoryData.slots[inventoryIndex].item;

            // 데이터 임시로 맡겨두기.
            UIManager.Instance.giveTemporaryItemData = inventory.slots[inventoryIndex].item;
            UIManager.Instance.giveTemporaryItemStack = inventory.slots[inventoryIndex].stack;
            inventory.slots[inventoryIndex].item = null;
            inventory.slots[inventoryIndex].stack = 0;

            itemImg.alpha = 0.6f;
            itemImg.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!inventory.slots[inventoryIndex].isEmpty)
        {
            uiItemTransform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(previousParent);
        uiItemTransform.position = previousParent.GetComponent<RectTransform>().position;

        itemImg.blocksRaycasts = true;

        // OnDrop이 먼저 발생하니, OnDrop이 발생하면 giveTemporaryItemData가 null이될거임.
        if (previousItem == UIManager.Instance.giveTemporaryItemData)
        {
            Debug.Log("원상복귀");
            inventory.slots[inventoryIndex].item = UIManager.Instance.giveTemporaryItemData;
            inventory.slots[inventoryIndex].stack = UIManager.Instance.giveTemporaryItemStack;
            UIManager.Instance.giveTemporaryItemData = null;
            UIManager.Instance.giveTemporaryItemStack = 0;
        }
        else
        {
            Debug.Log("데이터 옮겨짐");
            inventory.slots[inventoryIndex].item = UIManager.Instance.takeTemporaryItemData;
            inventory.slots[inventoryIndex].stack = UIManager.Instance.takeTemporaryItemStack;
            UIManager.Instance.takeTemporaryItemData = null;
            UIManager.Instance.takeTemporaryItemStack = 0;
        }

        inventory.StackUpdate(inventoryIndex);
    }
}