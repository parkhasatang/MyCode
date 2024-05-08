using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardQuestItemInfo : ItemInfo
{
    private Quest quest;

    protected override void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        quest = GetComponentInParent<Quest>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // itemInfo ÄÑÁÖ±â
        previousParent = itemInfo.transform.parent;
        itemInfo.transform.SetParent(canvas);
        itemInfo.transform.SetAsLastSibling();

        Item item = quest.questInfo.rewardItem;
        SetItemInfo(item);
        itemInfo.SetActive(true);
    }

    protected override void SetItemInfo(Item item)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"<b>{item.Name}</b>");
        sb.AppendLine();
        ItemEfficacy(item, sb);
        sb.AppendLine(item.Description);
        sb.AppendLine();

        itemInfoTxt.text = sb.ToString();
    }
}
