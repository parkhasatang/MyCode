using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public List<SlotData> slots = new();// SlotData�� ����Ʈ�� ���������.
    public SlotNum[] invenSlot;
    private EquipObject equipObject;

    public void Awake()
    {
        for(int i = 0; i < invenSlot.Length; i++)
        {
            SlotData slot = new()// ��ü �ʱ�ȭ �ܼ�ȭ
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

    // �κ��丮 �������� ���ð˻�.
    public bool CheckStackAmount(int itemCode, int requiredStack)
    {
        // �κ��丮 ��ĵ
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
        Debug.Log("�κ��丮�� �䱸���ǿ� �����ϴ� �������� ���� �����մϴ�.");
        return 0;
    }

    // �κ��丮 ���� �˻�.
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

    // �κ��丮���� ������ �������ֱ�.
    public void RemoveItemFromInventory(int itemCode, int stackNeed)
    {
        // �κ��丮 ��ĵ
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (slots[i].item != null && slots[i].item.ItemCode == itemCode)
            {
                // ������  ����� Stack�� ����� �ͺ��� ���� �� �����̵ɰŶ� ������ �� ������ ��� if������ �ȳ־��൵��.
                slots[i].stack -= stackNeed;
                StackUpdate(i);
                break;
            }
        }
    }

    public void GiveItemToEmptyInv(Item itemData, int stack)
    {
        // �κ��丮�� ��ġ�� �������� �ִ���.
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            // �κ��丮�� ����ִ���, �������� �������̶� ��ġ�ϴ���, �ִ밹������ ���� ����ִ���.
            if (!slots[i].isEmpty && slots[i].item.ItemCode == itemData.ItemCode && slots[i].stack < slots[i].item.StackNumber)
            {
                slots[i].stack += stack;
                // ������ ������ ��, �ִ밹���� ������.
                if (slots[i].stack > slots[i].item.StackNumber)
                {
                    int remainStack = slots[i].stack - slots[i].item.StackNumber;
                    slots[i].stack = slots[i].item.StackNumber;
                    // ���� ������ ���ļ� �Ʒ� for������ ��.
                    GiveItemToEmptyInv(itemData, remainStack);
                }
                StackUpdate(i);
                return;
            }
        }
        // ��ġ�� �������� ������.
        for (int i = 0; i < invenSlot.Length - 3; i++)
        {
            if (slots[i].isEmpty)
            {
                invenSlot[i].ChangeInventoryImage(itemData.ItemCode);
                invenSlot[i].OnOffImage(1f);
                slots[i].isEmpty = false;
                slots[i].item = itemData; // ������ �������� �����͸� �־��ش�.
                slots[i].stack = stack;
                StackUpdate(i);
                if (slots[i].isChoose)
                {
                    // �� ������ �������� ���� �տ� �̹��� ��Ÿ���� ����.
                    equipObject.heldItem.sprite = ItemManager.instance.GetSpriteByItemCode(itemData.ItemCode);
                }
                return;
            }
        }
    }
}
