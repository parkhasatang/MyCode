using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotNum : MonoBehaviour
{
    [SerializeField] internal Image itemImage;
    [SerializeField] private TMP_Text itemStack;

    public void ChangeInventoryImage(int itemCode)
    {
        itemImage.sprite = ItemManager.instance.GetSpriteByItemCode(itemCode);
    }

    public void OnOffImage(float value)
    {
        itemImage.GetComponent<CanvasGroup>().alpha = value;
    }

    public void QuickSlotItemChoose(bool isOn)
    {
        if (isOn)
        {
            Color imageColor = gameObject.GetComponent<Image>().color;
            imageColor.a = 1f;
            GetComponent<Image>().color = imageColor;
        }
        else
        {
            Color imageColor = gameObject.GetComponent<Image>().color;
            imageColor.a = 0f;
            GetComponent<Image>().color = imageColor;
        }
    }

    public void ItemStackUIRefresh(int stack)
    {
        if (stack <= 0)
        {
            itemStack.gameObject.SetActive(false);
            stack = 0;
        }
        else if (stack > 0)
        {
            // 1�� ���� ���� ���ֱ⸸ �ϱ�.
            if (stack == 1)
            {
                itemStack.gameObject.SetActive(false);
            }
            else
            {
                itemStack.gameObject.SetActive(true);
            }
            itemStack.text = $"{stack}";
        }
        // stack�� ������ �� ���� ����. 0�� �Ǹ� �����͸� ������ ���̱� ����.
    }
}
