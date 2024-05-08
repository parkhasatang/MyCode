using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    // 풀 담당 하는 리스트
    // 프리팹 갯수 = 리스트 갯수 맞춰줘야댐
    public GameObject[] prefabs;

    List<GameObject>[] pools;

    

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < pools.Length;  i++)
        {
            pools[i] = new List<GameObject>();
        }


    }

    /*select.transform.position = new Vector3(Random.Range(-8f, 8f), 6f, 5f);// select의 위치가 랜덤. select없으면 오브젝트 실시간으로 위치 변경됌*/
    public GameObject Get(int i)
    {
        GameObject select = null;
        // 선택한 풀의 비활성화 오브젝트에 접근
        foreach (GameObject item in pools[i]) 
        { 
            if (!item.activeSelf)
            {
                select = item;
                if (i == 2)
                {
                    // 바닥 똥
                    select.transform.position = new Vector3(-10f, -3.6f, 5f);
                }
                else
                {
                    //위에 똥, 알약들
                    select.transform.position = new Vector3(Random.Range(-8f, 8f), 6f, 5f);
                }
                select.SetActive(true);
                break;
            }
        }
        // 풀이 다 사용중이라면?
        if (!select)
        {
            // 새롭게 오브젝트를 생성하고 select 변수에 넣어주기
            select = Instantiate(prefabs[i], transform /*여기서 transform의 의미는 이 스크립트가 들어간 오브젝트 안에 자식 오브젝트로 생성을 한다는 의미다.*/);
            pools[i].Add(select);
        }

        return select;
    }
}
