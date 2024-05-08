using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public List<SlotData> slots = new();// SlotData를 리스트로 만들어주자.
    public SlotNum[] invenSlot;
    private EquipObject equipObject;

    public void Awake()
    {
        for(int i = 0; i < invenSlot.Length; i++)
        {
            SlotData slot = new()// 객체 초기화 단순화
            {
                isEmpty = true,
                isChoose = false,
                item = null,
                stack = 0
            };
            slots.Add(slot);
        }

        equipObject = GetComponent<EquipObject>();
    }

    // 인벤토리 아이템의 스택검사.
    public bool CheckStackAmount(int itemCode, int requiredStack)
    {
        // 인벤토리 스캔
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (!slots[i].isEmpty && slots[i].item != null)
            {
                if (slots[i].item.ItemCode == itemCode)
                {
                    if (slots[i].stack >= requiredStack)
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }
        return false;
    }

    public int ReturnStackInInventory(int itemCode)
    {
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (!slots[i].isEmpty && slots[i].item != null)
            {
                if (slots[i].item.ItemCode == itemCode)
                {
                    return slots[i].stack;
                }
            }
        }
        Debug.Log("인벤토리에 요구조건에 충족하는 아이템의 양이 부족합니다.");
        return 0;
    }

    // 인벤토리 스택 검사.
    public void StackUpdate(int indexOfInventory)
    {
        if (slots[indexOfInventory].stack == 0)
        {
            slots[indexOfInventory].item = null;
            slots[indexOfInventory].isEmpty = true;
            invenSlot[indexOfInventory].ChangeInventoryImage(0);
            invenSlot[indexOfInventory].OnOffImage(0);
        }
        else if (slots[indexOfInventory].stack > 0)
        {
            slots[indexOfInventory].isEmpty = false;
            invenSlot[indexOfInventory].ChangeInventoryImage(slots[indexOfInventory].item.ItemCode);
            invenSlot[indexOfInventory].OnOffImage(1f);
        }
        invenSlot[indexOfInventory].ItemStackUIRefresh(slots[indexOfInventory].stack);
    }

    // 인벤토리에서 아이템 제거해주기.
    public void RemoveItemFromInventory(int itemCode, int stackNeed)
    {
        // 인벤토리 스캔
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (slots[i].item != null && slots[i].item.ItemCode == itemCode)
            {
                // 어차피  재료의 Stack이 만드는 것보다 많을 때 실행이될거라 음수로 갈 걱정은 없어서 if문으로 안넣어줘도됌.
                slots[i].stack -= stackNeed;
                StackUpdate(i);
                break;
            }
        }
    }

    public void GiveItemToEmptyInv(Item itemData, int stack)
    {
        // 인벤토리에 겹치는 아이템이 있는지.
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            // 인벤토리가 비어있는지, 받을려는 아이템이랑 일치하는지, 최대갯수보다 적게 들어있는지.
            if (!slots[i].isEmpty && slots[i].item.ItemCode == itemData.ItemCode && slots[i].stack < slots[i].item.StackNumber)
            {
                slots[i].stack += stack;
                // 스택을 더했을 때, 최대갯수를 넘으면.
                if (slots[i].stack > slots[i].item.StackNumber)
                {
                    int remainStack = slots[i].stack - slots[i].item.StackNumber;
                    slots[i].stack = slots[i].item.StackNumber;
                    // 스택 갯수가 넘쳐서 아래 for문으로 들어감.
                    GiveItemToEmptyInv(itemData, remainStack);
                }
                StackUpdate(i);
                return;
            }
        }
        // 겹치는 아이템이 없으면.
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (slots[i].isEmpty)
            {
                invenSlot[i].ChangeInventoryImage(itemData.ItemCode);
                invenSlot[i].OnOffImage(1f);
                slots[i].isEmpty = false;
                slots[i].item = itemData; // 정해준 아이템의 데이터를 넣어준다.
                slots[i].stack = stack;
                StackUpdate(i);
                if (slots[i].isChoose)
                {
                    // 빈 곳으로 아이템이 들어가면 손에 이미지 나타나게 해줌.
                    equipObject.heldItem.sprite = ItemManager.instance.GetSpriteByItemCode(itemData.ItemCode);
                }
                return;
            }
        }
    }
}
