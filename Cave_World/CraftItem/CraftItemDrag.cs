using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftItemDrag : MonoBehaviour
{
    // �ν����� â���� ������ �ڵ� ������ߴ�.
    [SerializeField] internal int itemCode;
    // �巡�� ���� �� �갡 ���� ���������� �˰� �����͸� ����������ؼ�, ItemŬ������ �޾ƿ�.
    internal Item storeItemData;
    private Inventory inventory;
    internal bool isActive;

    private CanvasGroup itemImg;

    private CraftItemUI craftItemUI;

    private void Awake()
    {
        inventory = UIManager.Instance.playerInventoryData;
        craftItemUI = GetComponentInParent<CraftItemUI>();

        // ������ �������� ����
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
            // ������ ����
            CreateFromStore();

            UIManager.Instance.playerInventoryData.GiveItemToEmptyInv(storeItemData, 1);

            AudioManager.instance.PlaySffx(AudioManager.Sfx.ItemCrafting);

            itemImg.blocksRaycasts = false;

            // �ٽ� ��ĵ�ϱ�.
            craftItemUI.ReFreshCraftingUI();
        }
    }

    public void CreateFromStore()
    {
        // ���⿡ ���ս� �� �����ָ��.
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
