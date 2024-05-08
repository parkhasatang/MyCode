using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    public int itemCode;
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        item = ItemManager.instance.SetItemData(itemCode); // itemIndex로 아이템이 무엇인지 정해준다.
        if (collision.CompareTag("Player"))
        {
            UIManager.Instance.playerInventoryData.GiveItemToEmptyInv(item, 1);
            QuestManager.instance.CheckAllQuestRequest();
            gameObject.SetActive(false);
        }
    }
}
