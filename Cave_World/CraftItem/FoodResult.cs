using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodResult : MonoBehaviour
{
    private CanvasGroup itemImg;
    private Inventory inventory;

    private void Awake()
    {
        itemImg = GetComponent<CanvasGroup>();
        inventory = UIManager.Instance.playerInventoryData;
    }

    public void ClickButtonOnFood()
    {
        // 아이템 제작
        MakeFood();

        // 인벤토리로 보내기 시작. 데이터 임시보관소에 보내주기.
        UIManager.Instance.giveTemporaryItemData = inventory.slots[28].item;
        UIManager.Instance.giveTemporaryItemStack = 1;

        for (int i = 0; i < inventory.invenSlot.Length - 3; i++)
        {
            // 비어있거나 아이템 코드가 같다면
            if (inventory.slots[i].isEmpty || inventory.slots[i].item.ItemCode == inventory.slots[28].item.ItemCode)
            {
                inventory.slots[i].item = UIManager.Instance.giveTemporaryItemData;
                inventory.slots[i].stack += UIManager.Instance.giveTemporaryItemStack;
                inventory.StackUpdate(i);


                UIManager.Instance.giveTemporaryItemData = null;
                UIManager.Instance.giveTemporaryItemStack = 0;
                break;
                // 메서드가 종료되고싶은게 아니니깐 break;
            }
        }

        if ((inventory.slots[26].stack == 0) || (inventory.slots[27].stack == 0))
        {
            itemImg.blocksRaycasts = false;
            inventory.slots[28].stack = 0;
            inventory.StackUpdate(28);
        }
        // 아직 재료들이 남았다면
        else
        {
            itemImg.blocksRaycasts = true;
        }
    }


    /*public void OnBeginDrag(PointerEventData eventData)
    {


        // 안에 데이터들이 들어있다면
        if ((firstItem == 8) && (secondItem == 8))
        {
            // 해당 데이터의 Stack이 0보다 높으면
            if ((inventory.slots[26].stack > 0) && (inventory.slots[27].stack > 0))
            {
                Debug.Log("조합완료");
                FoodResultStack(true);
                // 임시저장소로 Item데이터 복사
                UIManager.Instance.giveTemporaryItemData = inventory.slots[inventoryIndex].item;
                // 임시저장소 스택 +1
                UIManager.Instance.giveTemporaryItemStack++;
                // 음식재료 스택이 0이면
                if ((inventory.slots[26].stack == 0) || (inventory.slots[27].stack == 0))
                {
                    inventory.slots[inventoryIndex].item = null;
                    inventory.slots[inventoryIndex].stack = 0;
                    UIManager.playerInventoryData.StackUpdate(inventoryIndex);
                }
                // 아직 재료들이 남았다면
                else
                {
                    inventory.slots[inventoryIndex].stack--;
                    UIManager.playerInventoryData.StackUpdate(inventoryIndex);
                }
            }
        }
    }*/

    private void MakeFood()
    {
        inventory.slots[26].stack--;
        inventory.slots[27].stack--;

        inventory.StackUpdate(26);
        inventory.StackUpdate(27);
        AudioManager.instance.PlaySffx(AudioManager.Sfx.MakeFood);
    }
}
