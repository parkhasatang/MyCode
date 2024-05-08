using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemInfo;
    public TMP_Text itemInfoTxt;
    protected DraggableUI draggableUI;

    protected Transform canvas;
    protected Transform previousParent;

    protected virtual void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        draggableUI = GetComponent<DraggableUI>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // itemInfo 켜주기
        previousParent = itemInfo.transform.parent;
        itemInfo.transform.SetParent(canvas);
        itemInfo.transform.SetAsLastSibling();

        int inventoryIndex = draggableUI.inventoryIndex;
        SlotData slots = UIManager.Instance.playerInventoryData.slots[inventoryIndex];

        if (!slots.isEmpty)
        {
            SetItemInfo(slots.item);
            itemInfo.SetActive(true);
        }
        else return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // itemInfo 꺼주기
        itemInfo.SetActive(false);
        itemInfo.transform.SetParent(previousParent);
    }

    protected virtual void SetItemInfo(Item item)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"<b>{item.Name}</b>");
        sb.AppendLine();
        ItemEfficacy(item, sb);
        sb.AppendLine(item.Description);

        itemInfoTxt.text = sb.ToString();
    }

    public void ItemEfficacy(Item item, StringBuilder sb)
    {
        bool hasStats = false;

        if (item.HP > 0)
        {
            if (item.ItemType != 1)
            {
                sb.AppendLine($"체력 : {item.HP}");
            }
            hasStats = true;
        }
        if (item.Hunger > 0)
        {
            sb.AppendLine($"배고픔 : {item.Hunger}");
            hasStats = true;
        }
        if (item.AttackDamage > 0)
        {
            if (item.ItemType == 12)
            {
                sb.AppendLine($"채굴피해 : {item.AttackDamage}");
            }
            else sb.AppendLine($"공격력 : {item.AttackDamage}");
            hasStats = true;
        }
        if (item.Defense > 0)
        {
            sb.AppendLine($"방어력 : {item.Defense}");
            hasStats = true;
        }
        if (item.Speed > 0)
        {
            sb.AppendLine($"이동속도 : {item.Speed}");
            hasStats = true;
        }
        if (hasStats)
        {
            sb.AppendLine();
        }
    }
}
