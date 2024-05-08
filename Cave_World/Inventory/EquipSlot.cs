using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item equipItemData;
    public int equipSlotStack;
    private Item previousItemData;

    [SerializeField] internal int receiveItemType;

    private SlotNum slotNum;

    private Sprite defaultSprite;

    private Transform canvas;
    private Transform previousParent;
    private RectTransform uiItemTransform;
    private CanvasGroup itemImg;

    public bool playerIsEquiped;

    private void Awake()
    {
        slotNum = GetComponentInParent<SlotNum>();

        canvas = FindObjectOfType<Canvas>().transform;
        uiItemTransform = GetComponent<RectTransform>();
        itemImg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetEquipDefaultImg();
        UIManager.Instance.UpdatePlayerStatTxt();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;
        transform.SetParent(canvas);
        transform.SetAsLastSibling();
        previousItemData = equipItemData;

        // ������ �ӽ÷� �ðܵα�.
        UIManager.Instance.giveTemporaryItemData = equipItemData;
        UIManager.Instance.giveTemporaryItemStack = equipSlotStack;
        equipItemData = null;
        equipSlotStack = 0;

        itemImg.alpha = 0.6f;
        itemImg.blocksRaycasts = false;        
    }

    public void OnDrag(PointerEventData eventData)
    {
        uiItemTransform.position = eventData.position;        
    }

    public void OnDrop(PointerEventData eventData)
    {
        
        if (UIManager.Instance.giveTemporaryItemData.ItemType == receiveItemType)
        {
            // ���� ������ �ø���
            UIManager.Instance.takeTemporaryItemData = equipItemData;
            UIManager.Instance.takeTemporaryItemStack = equipSlotStack;
            equipItemData = null;
            equipSlotStack = 0;

            // ������ �޾ƿ���.
            equipItemData = UIManager.Instance.giveTemporaryItemData;
            equipSlotStack = UIManager.Instance.giveTemporaryItemStack;
            UIManager.Instance.giveTemporaryItemData = null;
            UIManager.Instance.giveTemporaryItemStack = 0;

            itemImg.alpha = 1f;
            slotNum.ChangeInventoryImage(equipItemData.ItemCode);
            slotNum.ItemStackUIRefresh(equipSlotStack);            
            EquipManager.Instance.UpdatePlayerStat();           
            UIManager.Instance.UpdatePlayerStatTxt();
            AudioManager.instance.PlaySffx(AudioManager.Sfx.PlayerEquip);
        }
        // �´� Ÿ���� �������� ��Ӿ��Ѵٸ�.
        else
        {            
            // ���â�� �������� �ִٸ�
            if (equipSlotStack != 0)
            {
                itemImg.alpha = 1f;
            }
            else
            {
                // ���ٸ� �⺻�̹����� ������.
                slotNum.itemImage.sprite = defaultSprite;
                itemImg.alpha = 0.5f;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(previousParent);
        uiItemTransform.position = previousParent.GetComponent<RectTransform>().position;

        itemImg.blocksRaycasts = true;

        if (previousItemData == UIManager.Instance.giveTemporaryItemData)
        {
            equipItemData = UIManager.Instance.giveTemporaryItemData;
            equipSlotStack = UIManager.Instance.giveTemporaryItemStack;
            UIManager.Instance.giveTemporaryItemData = null;
            UIManager.Instance.giveTemporaryItemStack = 0;
        }
        else
        {
            equipItemData = UIManager.Instance.takeTemporaryItemData;
            equipSlotStack = UIManager.Instance.takeTemporaryItemStack;
            UIManager.Instance.takeTemporaryItemData = null;
            UIManager.Instance.takeTemporaryItemStack = 0;
        }
        // ���� ������Ʈ.
        if (equipItemData != null)
        {
            slotNum.ChangeInventoryImage(equipItemData.ItemCode);
            slotNum.ItemStackUIRefresh(equipSlotStack);
        }
        else
        {
            slotNum.itemImage.sprite = defaultSprite;
            itemImg.alpha = 0.5f;
        }        
    }

    private void SetEquipDefaultImg()
    {
        switch (receiveItemType)
        {
            case 3:
                slotNum.ChangeInventoryImage(1400);
                break;
            case 4:
                slotNum.ChangeInventoryImage(1500);
                break;
            case 5:
                slotNum.ChangeInventoryImage(1600);
                break;
            case 6:
                slotNum.ChangeInventoryImage(1800);
                break;
            case 7:
                slotNum.ChangeInventoryImage(1900);
                break;
        }
        defaultSprite = gameObject.GetComponent<Image>().sprite;
    }

}