using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftItemUI : MonoBehaviour
{
    // CanvasGroup�� BlockRaycats�� Ȱ���Ͽ� Drag & Drop���� ���������� BlockRaycats�� ���ָ� ������ �� �ְ� ���ش�.
    [SerializeField] private CanvasGroup[] craftItem;
    private Dictionary<int, CanvasGroup> craftItemOrder;

    internal List<int> stuffGather;
    internal int inventoryLength;
    private Inventory inventory;

    private void Awake()
    {
        inventory = UIManager.Instance.playerInventoryData;
        inventoryLength = inventory.invenSlot.Length - 3;
        craftItemOrder = new Dictionary<int, CanvasGroup>();

        // CraftItemDrag.cs���� �ν�����â���� ������ ItemCode�� Ű���� �����ϰ� �ش� �ε����� CanvasGroup�� �־��ش�.
        for (int i = 0; i < craftItem.Length; i++)
        {
            int craftItemDragItemCode = craftItem[i].GetComponent<CraftItemDrag>().itemCode;
            craftItemOrder.Add(craftItemDragItemCode, craftItem[i]);
        }
    }

    private void OnEnable()
    {
        // �κ��丮 �ѹ� ��ĵ�ϱ�
        ReFreshCraftingUI();
    }

    public void ReFreshCraftingUI()
    {
        // ���� �� stuffGather �ȿ� ����ֱ�
        stuffGather = new List<int>();

        for (int i = 0; i < inventoryLength; i++)
        {
            if (!UIManager.Instance.playerInventoryData.slots[i].isEmpty)
            {
                stuffGather.Add(UIManager.Instance.playerInventoryData.slots[i].item.ItemCode);
                Debug.Log("��ĵ�Ϸ�");
            }
            else continue;
        }

        for (int j = 0; j < craftItem.Length; j++)
        {
            craftItem[j].alpha = 0.2f;
            craftItem[j].GetComponent<CraftItemDrag>().isActive = false;
        }
        // ���ǿ� ������ ���۴뿡 �ִ� ������ ���ֱ�.
        Debug.Log("�ʱ�ȭ �Ϸ�");
        CraftingRecipe();
    }

    public void CraftingRecipe()
    {
        Debug.Log("������ ����");
        if (stuffGather.Contains(3011) && stuffGather.Contains(3101))
        {
            // ���� �ʿ䵵 ���� ���� ��, ������ Ȱ��ȭ
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 2))
            {
                Debug.Log("���� �� Ȱ��ȭ");
                SetCraftItemImage(1001);
            }
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("���� ��� Ȱ��ȭ");
                SetCraftItemImage(1301);
            }
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("���� ���� Ȱ��ȭ");
                SetCraftItemImage(1201);
            }

        }
        if (stuffGather.Contains(3101))
        {
            if (inventory.CheckStackAmount(3101, 4))
            {
                Debug.Log("���� ���� Ȱ��ȭ");
                SetCraftItemImage(1401);
            }
            if (inventory.CheckStackAmount(3101, 5))
            {
                Debug.Log("���� ���� Ȱ��ȭ");
                SetCraftItemImage(1501);
            }
            if (inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("���� �Ź� Ȱ��ȭ");
                SetCraftItemImage(1601);
            }
            if (inventory.CheckStackAmount(3101, 5))
            {
                Debug.Log("�� �Ѹ��� Ȱ��ȭ");
                SetCraftItemImage(1601);
            }
        }
        if (stuffGather.Contains(1001) && stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(1001, 1) && inventory.CheckStackAmount(3102, 4))
            {
                Debug.Log("ö�� Ȱ��ȭ");
                SetCraftItemImage(1002);
            }
        }
        if (stuffGather.Contains(1301) && stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(1301, 1) && inventory.CheckStackAmount(3102, 5))
            {
                Debug.Log("ö��� Ȱ��ȭ");
                SetCraftItemImage(1302);
            }
        }
        if (stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(3102, 4))
            {
                Debug.Log("ö ���� Ȱ��ȭ");
                SetCraftItemImage(1402);
            }
            if (inventory.CheckStackAmount(3102, 5))
            {
                Debug.Log("ö ���� Ȱ��ȭ");
                SetCraftItemImage(1502);
            }
            if (inventory.CheckStackAmount(3102, 3))
            {
                Debug.Log("ö �Ź� Ȱ��ȭ");
                SetCraftItemImage(1602);
            }
        }
    }


    private void SetCraftItemImage(int itemCode)
    {
        if (craftItemOrder.ContainsKey(itemCode))
        {
            craftItemOrder[itemCode].alpha = 1f;
            craftItemOrder[itemCode].GetComponent<CraftItemDrag>().isActive = true;
        }
        else Debug.Log("�������´� ����");
    }
}
