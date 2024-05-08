using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int itemCount;
    public Image slotImage;
    public Item item;
    [SerializeField] private Sprite defaultSprite;

    [SerializeField] private TMP_Text slotItemCountTxt;

    private void Awake()
    {
        slotImage = GetComponent<Image>();
    }

    public void AddItem(Item _item, int _itemCount = 1)// ���ο� �������� �߰��� ��.
    {
        itemCount += _itemCount;
        item = _item;
        /*slotImage = item.Image;*/
    }

    public void UpdateItemCount(int _count)// �������� ī��Ʈ�� �÷���.
    {
        itemCount += _count;
        slotItemCountTxt.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        slotImage.sprite = defaultSprite;
    }
}
