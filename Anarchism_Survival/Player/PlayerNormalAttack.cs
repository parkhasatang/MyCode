using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalAttack : MonoBehaviour
{
    public Transform firePoint; // �Ѿ��� �߻�� ��ġ
    public float bulletSpeed; 
    public float attackRange = 8f; // ���� ����
    

    protected float attackTimer = 0f; // ���� ���ݱ��� ���� �ð��� ����

    public int prefabId; // �ν�����â���� �´� ������ ����

    private void Awake()
    {
        firePoint = GetComponent<Transform>();
    }

    private void Update()
    {
        if (attackTimer <= 0f)
        {
            ShootNearestEnemy();
            attackTimer = GameManager.Instance.player.attckSpeed;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    

    protected void ShootNearestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = hitCollider.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            Vector2 direction = (nearestEnemy.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject bullet = PoolManager.instance.GetPool(prefabId);// �Ѿ� Prefab
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);
            }
            bullet.GetComponent<Bullet>().damage = GetAttackDamage();
        }
    }

    protected virtual int GetAttackDamage()
    {
        return GameManager.Instance.player.attackDamage;
    }
}
