using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public Transform[] poolSpawnPositions;

    // ��� �������� ����
    public GameObject[] prefabs;

    // ��� Ǯ�� ����
    public List<GameObject>[] pools;

    private void Awake()
    {
        instance = this;

        pools = new List<GameObject>[prefabs.Length];
        
        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetPool(int index)
    {
        GameObject prefab = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeInHierarchy)
            {
                prefab = item;
                prefab.SetActive(true);
                break;
            }
        }

        if (!prefab)
        {
            prefab = Instantiate(prefabs[index], poolSpawnPositions[index]);
            pools[index].Add(prefab);
        }

        return prefab;
    }
}
