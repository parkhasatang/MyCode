using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftItemInfo : ItemInfo
{
    private CraftItemDrag craftItemDrag; 
    protected override void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        craftItemDrag = GetComponent<CraftItemDrag>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // itemInfo ÄÑÁÖ±â
        previousParent = itemInfo.transform.parent;
        itemInfo.transform.SetParent(canvas);
        itemInfo.transform.SetAsLastSibling();

        Item item = craftItemDrag.storeItemData;
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
        CraftRecipe(item, sb);

        itemInfoTxt.text = sb.ToString();
    }

    public void CraftRecipe(Item item, StringBuilder sb)
    {
        Dictionary<int, int> materials = ItemManager.instance.GetCraftingRecipe(item.ItemCode);

        if (materials != null)
        {
            foreach (var material in materials)
            {
                string materialName = ItemManager.instance.items[material.Key].Name;
                sb.Append($"{materialName} x {material.Value}\n");
            }
        }
    }
}
