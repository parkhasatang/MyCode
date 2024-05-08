using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemImage : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = ItemManager.instance.GetSpriteByItemCode(GetComponent<PickUp>().itemCode);
    }
}
