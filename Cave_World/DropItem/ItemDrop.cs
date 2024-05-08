using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemDrop : MonoBehaviour
{
    private void Start()
    {
        ItemManager.instance.itemPool.ItemSpawn(2101, transform.position);
    }
}
