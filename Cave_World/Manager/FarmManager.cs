using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager instance;

    public GameObject fieldPrefab;
    private List<GameObject> fieldPool;

    private void Awake()
    {
        instance = this;

        fieldPool = new List<GameObject>();
    }

    public void FieldSpawn(Vector3 spawnPosition)
    {
        GameObject selectedFieldPrefab = null;
        foreach (GameObject field in fieldPool)
        {
            if (!field.activeSelf)
            {
                selectedFieldPrefab = field;
                selectedFieldPrefab.transform.position = spawnPosition;
                selectedFieldPrefab.SetActive(true);
                break;
            }
        }

        if (!selectedFieldPrefab)
        {
            selectedFieldPrefab = Instantiate(fieldPrefab, transform);
            selectedFieldPrefab.transform.position = spawnPosition;
            fieldPool.Add(selectedFieldPrefab);
        }
    }
}
