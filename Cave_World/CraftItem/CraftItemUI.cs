using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftItemUI : MonoBehaviour
{
    // CanvasGroup의 BlockRaycats를 활용하여 Drag & Drop으로 가져갈려고 BlockRaycats만 켜주면 가져갈 수 있게 해준다.
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

        // CraftItemDrag.cs에서 인스펙터창에서 정해진 ItemCode를 키값에 저장하고 해당 인덱스의 CanvasGroup을 넣어준다.
        for (int i = 0; i < craftItem.Length; i++)
        {
            int craftItemDragItemCode = craftItem[i].GetComponent<CraftItemDrag>().itemCode;
            craftItemOrder.Add(craftItemDragItemCode, craftItem[i]);
        }
    }

    private void OnEnable()
    {
        // 인벤토리 한번 스캔하기
        ReFreshCraftingUI();
    }

    public void ReFreshCraftingUI()
    {
        // 켜질 때 stuffGather 안에 비워주기
        stuffGather = new List<int>();

        for (int i = 0; i < inventoryLength; i++)
        {
            if (!UIManager.Instance.playerInventoryData.slots[i].isEmpty)
            {
                stuffGather.Add(UIManager.Instance.playerInventoryData.slots[i].item.ItemCode);
                Debug.Log("스캔완료");
            }
            else continue;
        }

        for (int j = 0; j < craftItem.Length; j++)
        {
            craftItem[j].alpha = 0.2f;
            craftItem[j].GetComponent<CraftItemDrag>().isActive = false;
        }
        // 조건에 맞으면 제작대에 있는 아이템 켜주기.
        Debug.Log("초기화 완료");
        CraftingRecipe();
    }

    public void CraftingRecipe()
    {
        Debug.Log("레시피 검토");
        if (stuffGather.Contains(3011) && stuffGather.Contains(3101))
        {
            // 스택 필요도 보다 많을 때, 아이템 활성화
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 2))
            {
                Debug.Log("구리 검 활성화");
                SetCraftItemImage(1001);
            }
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("구리 곡괭이 활성화");
                SetCraftItemImage(1301);
            }
            if (inventory.CheckStackAmount(3011, 1) && inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("구리 괭이 활성화");
                SetCraftItemImage(1201);
            }

        }
        if (stuffGather.Contains(3101))
        {
            if (inventory.CheckStackAmount(3101, 4))
            {
                Debug.Log("구리 투구 활성화");
                SetCraftItemImage(1401);
            }
            if (inventory.CheckStackAmount(3101, 5))
            {
                Debug.Log("구리 갑옷 활성화");
                SetCraftItemImage(1501);
            }
            if (inventory.CheckStackAmount(3101, 3))
            {
                Debug.Log("구리 신발 활성화");
                SetCraftItemImage(1601);
            }
            if (inventory.CheckStackAmount(3101, 5))
            {
                Debug.Log("물 뿌리개 활성화");
                SetCraftItemImage(1601);
            }
        }
        if (stuffGather.Contains(1001) && stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(1001, 1) && inventory.CheckStackAmount(3102, 4))
            {
                Debug.Log("철검 활성화");
                SetCraftItemImage(1002);
            }
        }
        if (stuffGather.Contains(1301) && stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(1301, 1) && inventory.CheckStackAmount(3102, 5))
            {
                Debug.Log("철곡괭이 활성화");
                SetCraftItemImage(1302);
            }
        }
        if (stuffGather.Contains(3102))
        {
            if (inventory.CheckStackAmount(3102, 4))
            {
                Debug.Log("철 투구 활성화");
                SetCraftItemImage(1402);
            }
            if (inventory.CheckStackAmount(3102, 5))
            {
                Debug.Log("철 갑옷 활성화");
                SetCraftItemImage(1502);
            }
            if (inventory.CheckStackAmount(3102, 3))
            {
                Debug.Log("철 신발 활성화");
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
        else Debug.Log("지원스승님 만세");
    }
}
