using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopController : MonoBehaviour
{
    // ����� ���� ��� ���ߴ��� ī��Ʈ ���ְ� �� �� ��ŭ �� ������ ���� ���ش�.
    // �׷��� Collision���� ī��Ʈ �޾ƿ���, Spawn���� ���������ֱ�.

    public int count = 0;
    PoopSpawn _PoopSpawn;

    void Start()
    {
        _PoopSpawn = FindObjectOfType<PoopSpawn>();
    }



    void Update()
    {

        if (count == 20)
        {
            _PoopSpawn.spawnRate += 0.2f;
            count = 0;
            Debug.Log("������ ����!");
        }
    }
}
