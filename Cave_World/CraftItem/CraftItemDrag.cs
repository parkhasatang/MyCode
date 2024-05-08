using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftItemDrag : MonoBehaviour
{
    // 인스펙터 창에서 아이템 코드 적어줘야댐.
    [SerializeField] internal int itemCode;
    // 드래그 했을 때 얘가 무슨 아이템인지 알고 데이터를 전달해줘야해서, Item클래스를 받아옴.
    internal Item storeItemData;
    private Inventory inventory;
    internal bool isActive;

    private CanvasGroup itemImg;

    private CraftItemUI craftItemUI;

    private void Awake()
    {
        inventory = UIManager.Instance.playerInventoryData;
        craftItemUI = GetComponentInParent<CraftItemUI>();

        // 아이템 무엇인지 결정
        storeItemData = ItemManager.instance.SetItemData(itemCode);

        itemImg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        gameObject.GetComponent<Image>().sprite = ItemManager.instance.GetSpriteByItemCode(itemCode);
    }

    public void ClickButtonOnStore()
    {
        if (isActive)
        {
            // 아이템 제작
            CreateFromStore();

            UIManager.Instance.playerInventoryData.GiveItemToEmptyInv(storeItemData, 1);

            AudioManager.instance.PlaySffx(AudioManager.Sfx.ItemCrafting);

            itemImg.blocksRaycasts = false;

            // 다시 스캔하기.
            craftItemUI.ReFreshCraftingUI();
        }
    }

    public void CreateFromStore()
    {
        // 여기에 조합식 다 적어주면됌.
        switch (itemCode)
        {
            case 1001:
                inventory.RemoveItemFromInventory(3011, 1);
                inventory.RemoveItemFromInventory(3101, 2);
                break;
            case 1301:
                inventory.RemoveItemFromInventory(3011, 1);
                inventory.RemoveItemFromInventory(3101, 3);
                break;
            case 1401:
                inventory.RemoveItemFromInventory(3101, 4);
                break;
            case 1501:
                inventory.RemoveItemFromInventory(3101, 5);
                break;
            case 1601:
                inventory.RemoveItemFromInventory(3101, 3);
                break;
            case 1002:
                inventory.RemoveItemFromInventory(1001, 1);
                inventory.RemoveItemFromInventory(3102, 4);
                break;
            case 1302:
                inventory.RemoveItemFromInventory(1301, 1);
                inventory.RemoveItemFromInventory(3102, 5);
                break;
            case 1402:
                inventory.RemoveItemFromInventory(3102, 4);
                break;
            case 1502:
                inventory.RemoveItemFromInventory(3102, 5);
                break;
            case 1602:
                inventory.RemoveItemFromInventory(3102, 3);
                break;
            case 1201:
                inventory.RemoveItemFromInventory(3011, 1);
                inventory.RemoveItemFromInventory(3101, 3);
                break;
            case 1251:
                inventory.RemoveItemFromInventory(3101, 5);
                break;
        }
    }
}
