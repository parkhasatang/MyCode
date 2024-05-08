using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSpawn : MonoBehaviour
{

    // ������ ���۵Ǹ� ���� �����ϰ� ������ ������ش�.

    Vector3 spawnPosition;


    float nextSpawnTime = 0f;
    public float spawnRate = 2f;// �� ���� �����Ͽ� �� ���� ������ ����.

    void Update()
    {

        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + 1f / spawnRate;
            SpawnPoop();
            SpawnSpeedPoop();
            SpawnBottomPoop();
            SpawnBluePill();
        }

    }

    //���⿡ Instantiate�� �� ���� �޼��� �����

    // �˵� ó�� ���� ��ġ
    void SpawnPoop()
    {
        GameObject poop = GameManager.I.pool.Get(0);
        spawnPosition = new Vector3(Random.Range(-8f, 8f), 6f, 5f);
        poop.transform.position = spawnPosition;
    }

    void SpawnSpeedPoop()
    {
        // 0�� 1 ������ ������ �� ����
        float randomValue = Random.value;
        if (randomValue < 0.4f)
        {
            GameObject poop = GameManager.I.pool.Get(1);
            spawnPosition = new Vector3(Random.Range(-8f, 8f), 6f, 5f);
            poop.transform.position = spawnPosition;
        }
    }

    void SpawnBottomPoop()
    {
        float randomValue = Random.value;
        if (randomValue < 0.15f)
        {
            GameObject poop = GameManager.I.pool.Get(2);
            spawnPosition = new Vector3(-10f, -3.65f, 5f);
            poop.transform.position = spawnPosition;
        }
    }

    // �˾�� ó�� ���� ��ġ
    void SpawnBluePill()
    {
        float randomValue = Random.value;
        if (randomValue < 0.1f)
        {
            GameObject poop = GameManager.I.pool.Get(3);
            spawnPosition = new Vector3(Random.Range(-8f, 8f), 6f, 5f);
            poop.transform.position = spawnPosition;
        }
    }
}