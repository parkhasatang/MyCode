using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class Item
{
    public Item(int itemCode, int itemType, string name, string description, float Hp, float hunger, float attackDamage, float attackDelay, float denfense,
        float attackRange, float speed, int stackNumber, bool isEquip, bool rightClick)
    {
        ItemCode = itemCode; ItemType = itemType; Name = name; Description = description; HP = Hp; Hunger = hunger; AttackDamage = attackDamage; AttackRange = attackDelay;
        Defense = denfense; Speed = speed; StackNumber = stackNumber; IsEquip = isEquip; RightClick = rightClick;
    }
    public int ItemCode, ItemType, StackNumber;
    public float HP, Hunger, AttackDamage, AttackDelay, Defense, AttackRange, Speed;
    public string Name, Description;
    public bool IsEquip, RightClick;
}

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;

    public static ItemManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemManager>();

                if (_instance == null)
                {
                    GameObject ItemManager = new GameObject("ItemManager");
                    _instance = ItemManager.AddComponent<ItemManager>();
                }
            }
            return _instance;
        }
    }

    public TextAsset ItemDatas;
    public Dictionary<int, Item> items = new Dictionary<int, Item>() { };

    public ItemPool itemPool;

    public Dictionary<int, Sprite> spriteDictionary;
    private Dictionary<int, Dictionary<int, int>> craftingRecipes;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        _instance = this;

        spriteDictionary = new Dictionary<int, Sprite>();
        craftingRecipes = new Dictionary<int, Dictionary<int, int>>();

        ItemMapping();

        SpriteMapping();

        // 무기 제작 레시피 초기화
        InitializeCraftingRecipes();
    }

    private void ItemMapping()
    {
        string[] line = ItemDatas.text.Substring(0, ItemDatas.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            items.Add(int.Parse(row[0]), new Item(int.Parse(row[0]), int.Parse(row[1]), row[2], row[3], float.Parse(row[4]), float.Parse(row[5]), float.Parse(row[6]),
                float.Parse(row[7]), float.Parse(row[8]), float.Parse(row[9]), float.Parse(row[10]), int.Parse(row[11]), bool.Parse(row[12]), bool.Parse(row[13])));
        }
    }


    public Sprite GetSpriteByItemCode(int itemCode)
    {
        // Dictionary에서 해당 인트 값에 대응하는 Sprite를 가져오기
        if (spriteDictionary.TryGetValue(itemCode, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.Log("해당 키값에 이미지없음.");
            return null;
        }
    }

    public Item SetItemData(int itemCode)
    {
        return items[itemCode];
    }

    public void SpriteMapping()
    {
        // Resource폴더안 ItemSprite폴더안의 모든 파일을 긁어와서 배열에 저장.
        Sprite[] itemSprite = Resources.LoadAll<Sprite>("ItemSprite");

        for (int i = 0; i < itemSprite.Length; i++)
        {
            int itemCode = int.Parse(itemSprite[i].name);
            spriteDictionary.Add(itemCode, itemSprite[i]);
        }
        Debug.Log("이미지 로드 완료");
    }

    // 램덤으로 요구 아이템 생성
    public Item CreateRandomItemByType(int itemType)
    {
        // 리스트 비워주기.
        List<Item> requestItemList = new List<Item>();

        // ItemManager에 있는 아이템 중에서 ItemType이 8인 아이템을 List에 넣어주기.
        foreach (Item item in items.Values)
        {
            if (item.ItemType == itemType)
            {
                requestItemList.Add(item);
            }
        }

        // 후보 아이템 중에서 랜덤하게 선택하여 requestItem에 넣어주기.
        Item resultItem;

        if (requestItemList.Count > 0)
        {
            resultItem = requestItemList[Random.Range(0, requestItemList.Count)];
        }
        else
        {
            resultItem = null;
        }
        return resultItem;
    }

    

    private void InitializeCraftingRecipes()
    {
        // 아이템 코드와 해당하는 아이템 재료 및 필요 갯수 추가
        AddCraftingRecipe(1001, new Dictionary<int, int> { { 3011, 1 }, { 3101, 2 } });
        AddCraftingRecipe(1301, new Dictionary<int, int> { { 3011, 1 }, { 3101, 3 } });
        AddCraftingRecipe(1401, new Dictionary<int, int> { { 3101, 4 } });
        AddCraftingRecipe(1501, new Dictionary<int, int> { { 3101, 5 } });
        AddCraftingRecipe(1601, new Dictionary<int, int> { { 3101, 3 } });
        AddCraftingRecipe(1002, new Dictionary<int, int> { { 1001, 1 }, { 3102, 4 } });
        AddCraftingRecipe(1302, new Dictionary<int, int> { { 1301, 1 }, { 3102, 5 } });
        AddCraftingRecipe(1402, new Dictionary<int, int> { { 3102, 4 } });
        AddCraftingRecipe(1502, new Dictionary<int, int> { { 3102, 5 } });
        AddCraftingRecipe(1602, new Dictionary<int, int> { { 3102, 3 } });
    }

    private void AddCraftingRecipe(int itemCode, Dictionary<int, int> materials)
    {
        craftingRecipes[itemCode] = materials;
    }

    public Dictionary<int, int> GetCraftingRecipe(int itemCode)
    {
        // 무기 제작 레시피 반환
        return craftingRecipes.ContainsKey(itemCode) ? craftingRecipes[itemCode] : null;
    }
}
