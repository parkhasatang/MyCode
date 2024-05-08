using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float spawnInterval;
    public List<EnemyData> enemyDatas = new List<EnemyData>();
    public int numEnemy;

    public int enemyPrefabId;

    private float gameTime = 60f;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesEverySecond());
        StartCoroutine(GameTime());
    }

    IEnumerator SpawnEnemiesEverySecond()
    {
        while (true)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                SpawnEnemyAtPoint(spawnPoint.position);
            }
            yield return new WaitForSeconds(spawnInterval);  // 1초마다 반복
        }
    }

    private void SpawnEnemyAtPoint(Vector3 position)
    {
        GameObject enemy = PoolManager.instance.GetPool(enemyPrefabId);

        enemy.GetComponent<Enemy>().SetEnemyStats();
        enemy.transform.position = position;
        enemy.SetActive(true);
    }
    IEnumerator GameTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameTime);
            numEnemy++;
            PoolManager.instance.prefabs[enemyPrefabId].GetComponent<Enemy>().data = enemyDatas[numEnemy];
            PoolManager.instance.prefabs[enemyPrefabId].GetComponent<Enemy>().SetEnemyStats();
        }
    }

}
