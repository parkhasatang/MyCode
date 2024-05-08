using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int attackDamage; // 플레이어의 기본 공격력 설정
    public float attckSpeed;

    void Start()
    {
        InitializeStats();
    }

    void InitializeStats()
    {
        // 플레이어의 공격력을 초기화할 수 있는 메소드
        attackDamage = 100; // 매 게임 시작마다 공격력을 기본 값으로 초기화
        attckSpeed = 2f;
    }

    public void UpgradeDamage()
    {
        attackDamage = 100 + ((GameManager.Instance.level - 1) * 10);
    }
}
