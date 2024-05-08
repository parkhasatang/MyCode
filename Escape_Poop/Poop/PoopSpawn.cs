using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSpawn : MonoBehaviour
{

    // 게임이 시작되면 똥이 랜덤하게 나오게 만들어준다.

    Vector3 spawnPosition;


    float nextSpawnTime = 0f;
    public float spawnRate = 2f;// 이 값을 조절하여 똥 생성 간격을 조정.

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

    //여기에 Instantiate로 똥 생성 메서드 만들기

    // 똥들 처음 스폰 위치
    void SpawnPoop()
    {
        GameObject poop = GameManager.I.pool.Get(0);
        spawnPosition = new Vector3(Random.Range(-8f, 8f), 6f, 5f);
        poop.transform.position = spawnPosition;
    }

    void SpawnSpeedPoop()
    {
        // 0과 1 사이의 랜덤한 값 생성
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

    // 알약들 처음 스폰 위치
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