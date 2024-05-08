using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public GameObject itemPrefab;
    private List<GameObject> itemPool;


    private void Awake()
    {
        itemPool = new List<GameObject>();
    }    

    public void ItemSpawn(int itemCode, Vector3 spawnPosition)
    {
        GameObject selectItemPrefab = null;
        foreach (GameObject item in itemPool)
        {
            if (!item.activeSelf)
            {
                selectItemPrefab = item;
                var pickUpComponent = selectItemPrefab.GetComponent<PickUp>();
                pickUpComponent.itemCode = itemCode;

                selectItemPrefab.GetComponent<SpriteRenderer>().sprite = ItemManager.instance.GetSpriteByItemCode(itemCode);
                selectItemPrefab.transform.position = spawnPosition;
                selectItemPrefab.SetActive(true);
                break;
            }
        }

        if (!selectItemPrefab)
        {
            selectItemPrefab = Instantiate(itemPrefab, transform);
            selectItemPrefab.transform.position = spawnPosition;
            var pickUpComponent = selectItemPrefab.GetComponent<PickUp>();
            pickUpComponent.itemCode = itemCode;

            selectItemPrefab.GetComponent<SpriteRenderer>().sprite = ItemManager.instance.GetSpriteByItemCode(itemCode);
            itemPool.Add(selectItemPrefab);
        }
    }
}
