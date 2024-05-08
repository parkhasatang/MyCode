using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int attackDamage; // �÷��̾��� �⺻ ���ݷ� ����
    public float attckSpeed;

    void Start()
    {
        InitializeStats();
    }

    void InitializeStats()
    {
        // �÷��̾��� ���ݷ��� �ʱ�ȭ�� �� �ִ� �޼ҵ�
        attackDamage = 100; // �� ���� ���۸��� ���ݷ��� �⺻ ������ �ʱ�ȭ
        attckSpeed = 2f;
    }

    public void UpgradeDamage()
    {
        attackDamage = 100 + ((GameManager.Instance.level - 1) * 10);
    }
}
