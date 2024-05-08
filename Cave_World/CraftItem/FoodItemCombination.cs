using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodItemCombination : MonoBehaviour, IBeginDragHandler, IDropHandler, IEndDragHandler
{
    private Dictionary<Tuple<int, int>, Item> foodRecipe;


    private void Start()
    {
        StartCoroutine(FoodRecipe());
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("조합 검토");
        CheckRecipe();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.Instance.playerInventoryData.slots[28].item = null;
        UIManager.Instance.playerInventoryData.slots[28].stack = 0;
        UIManager.Instance.playerInventoryData.StackUpdate(28);
        UIManager.Instance.playerInventoryData.invenSlot[28].GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        CheckRecipe();
    }

    public void CheckRecipe()
    {
        if (UIManager.Instance.playerInventoryData.slots[26]?.item != null && UIManager.Instance.playerInventoryData.slots[27]?.item != null)
        {
            // 음식 조합의 칸 두개의 Item데이터가 비어있지 않을 때
            if ((UIManager.Instance.playerInventoryData.slots[26].item.ItemType == 8) && (UIManager.Instance.playerInventoryData.slots[27].item.ItemType == 8))
            {
                int ingredient1 = UIManager.Instance.playerInventoryData.slots[26].item.ItemCode;
                int ingredient2 = UIManager.Instance.playerInventoryData.slots[27].item.ItemCode;

                Tuple<int, int> key = Tuple.Create(ingredient1, ingredient2);


                if (foodRecipe.ContainsKey(key))
                {
                    Item resultFood = foodRecipe[key];
                    // 데이터 등록
                    UIManager.Instance.playerInventoryData.slots[28].item = resultFood;
                    UIManager.Instance.playerInventoryData.slots[28].isEmpty = false;
                    // 등록된 데이터의 ItemCode로 이미지 불러오기.
                    UIManager.Instance.playerInventoryData.slots[28].stack++;
                    UIManager.Instance.playerInventoryData.StackUpdate(28);

                    UIManager.Instance.playerInventoryData.invenSlot[28].GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
                }
                else
                {
                    Debug.Log("레시피 없음");
                }
            }
            else
            {
                UIManager.Instance.playerInventoryData.slots[28].item = null;
                UIManager.Instance.playerInventoryData.slots[28].stack = 0;
                UIManager.Instance.playerInventoryData.StackUpdate(28);
                UIManager.Instance.playerInventoryData.invenSlot[28].GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
            }
        }
        else
        {            
            UIManager.Instance.playerInventoryData.slots[28].item = null;
            UIManager.Instance.playerInventoryData.slots[28].stack = 0;
            UIManager.Instance.playerInventoryData.StackUpdate(28);
            UIManager.Instance.playerInventoryData.invenSlot[28].GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
        }
    }


    public IEnumerator FoodRecipe()
    {
        yield return new WaitForSeconds(1f);

        foodRecipe = new Dictionary<Tuple<int, int>, Item>();
        foodRecipe.Add(Tuple.Create(1712, 1712), ItemManager.instance.items[3710]);
        foodRecipe.Add(Tuple.Create(1713, 1712), ItemManager.instance.items[3709]);
        foodRecipe.Add(Tuple.Create(1712, 1713), ItemManager.instance.items[3709]);
        foodRecipe.Add(Tuple.Create(1713, 1713), ItemManager.instance.items[3708]);
        foodRecipe.Add(Tuple.Create(1714, 1712), ItemManager.instance.items[3707]);
        foodRecipe.Add(Tuple.Create(1712, 1714), ItemManager.instance.items[3707]);
        foodRecipe.Add(Tuple.Create(1713, 1714), ItemManager.instance.items[3706]);
        foodRecipe.Add(Tuple.Create(1714, 1713), ItemManager.instance.items[3706]);
        foodRecipe.Add(Tuple.Create(1714, 1714), ItemManager.instance.items[3705]);
        foodRecipe.Add(Tuple.Create(1715, 1712), ItemManager.instance.items[3704]);
        foodRecipe.Add(Tuple.Create(1712, 1715), ItemManager.instance.items[3704]);
        foodRecipe.Add(Tuple.Create(1713, 1715), ItemManager.instance.items[3703]);
        foodRecipe.Add(Tuple.Create(1715, 1713), ItemManager.instance.items[3703]);
        foodRecipe.Add(Tuple.Create(1715, 1714), ItemManager.instance.items[3702]);
        foodRecipe.Add(Tuple.Create(1714, 1715), ItemManager.instance.items[3702]);
        foodRecipe.Add(Tuple.Create(1715, 1715), ItemManager.instance.items[3701]);
    }

}
