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
        // ������ ����
        MakeFood();

        // �κ��丮�� ������ ����. ������ �ӽú����ҿ� �����ֱ�.
        UIManager.Instance.giveTemporaryItemData = inventory.slots[28].item;
        UIManager.Instance.giveTemporaryItemStack = 1;

        for (int i = 0; i < inventory.invenSlot.Length - 3; i++)
        {
            // ����ְų� ������ �ڵ尡 ���ٸ�
            if (inventory.slots[i].isEmpty || inventory.slots[i].item.ItemCode == inventory.slots[28].item.ItemCode)
            {
                inventory.slots[i].item = UIManager.Instance.giveTemporaryItemData;
                inventory.slots[i].stack += UIManager.Instance.giveTemporaryItemStack;
                inventory.StackUpdate(i);


                UIManager.Instance.giveTemporaryItemData = null;
                UIManager.Instance.giveTemporaryItemStack = 0;
                break;
                // �޼��尡 ����ǰ������ �ƴϴϱ� break;
            }
        }

        if ((inventory.slots[26].stack == 0) || (inventory.slots[27].stack == 0))
        {
            itemImg.blocksRaycasts = false;
            inventory.slots[28].stack = 0;
            inventory.StackUpdate(28);
        }
        // ���� ������ ���Ҵٸ�
        else
        {
            itemImg.blocksRaycasts = true;
        }
    }


    /*public void OnBeginDrag(PointerEventData eventData)
    {


        // �ȿ� �����͵��� ����ִٸ�
        if ((firstItem == 8) && (secondItem == 8))
        {
            // �ش� �������� Stack�� 0���� ������
            if ((inventory.slots[26].stack > 0) && (inventory.slots[27].stack > 0))
            {
                Debug.Log("���տϷ�");
                FoodResultStack(true);
                // �ӽ�����ҷ� Item������ ����
                UIManager.Instance.giveTemporaryItemData = inventory.slots[inventoryIndex].item;
                // �ӽ������ ���� +1
                UIManager.Instance.giveTemporaryItemStack++;
                // ������� ������ 0�̸�
                if ((inventory.slots[26].stack == 0) || (inventory.slots[27].stack == 0))
                {
                    inventory.slots[inventoryIndex].item = null;
                    inventory.slots[inventoryIndex].stack = 0;
                    UIManager.playerInventoryData.StackUpdate(inventoryIndex);
                }
                // ���� ������ ���Ҵٸ�
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
