using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttacking : MonoBehaviour
{
    Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
    }

    public IEnumerator Rotate()
    {
        // ���� ������ ���� ������Ʈ�� ��� ȸ����ŵ�ϴ�.
        while (true)
        {
            // Vector3.back�� ����� Z���� �������� ȸ��
            // 150�� * Time.deltaTime�� ����Ͽ� ������ �ӵ��� ������� ������ �ӵ��� ȸ��
            transform.Rotate(Vector3.back * 150 * Time.deltaTime);

            // ���� �����ӱ��� ������ ����
            yield return null;
        }
    }

    public IEnumerator Shoot(float interval)
    {
        while (true)
        {
            int count = weapon.count;
            for (int i = 0; i < count; i++)
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(GameManager.Instance.playerPosition.position, 10);
                Transform nearestEnemy = null;
                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Boss"))
                    {
                        nearestEnemy = hitCollider.transform;
                    }
                    else if (hitCollider.CompareTag("Enemy"))
                    {
                        nearestEnemy = hitCollider.transform;
                    }
                }

                if (nearestEnemy != null)
                {
                    Vector2 direction = (nearestEnemy.position - GameManager.Instance.playerPosition.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    GameObject bullet = PoolManager.instance.GetPool(weapon.prefabId);
                    bullet.transform.position = GameManager.Instance.playerPosition.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.AddForce(direction * 20, ForceMode2D.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public IEnumerator RandomThrow(float interval)
    {
        while (true)
        {
            int count = weapon.count;
            for (int i = 0; i < count; i++)
            {
                GameObject bullet = PoolManager.instance.GetPool(weapon.prefabId);
                bullet.transform.position = GameManager.Instance.playerPosition.position;
                // ������ ������ ���
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                Vector3 throwDirection = new Vector3(randomDirection.x, randomDirection.y, 0);

                // Rigidbody ������Ʈ�� ����Ͽ� ���������� ȭ���� �߻�
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(throwDirection * 3, ForceMode2D.Impulse);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void Batch()// ���� �������� ��ȯ.
    {
         int count = weapon.count;

        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = PoolManager.instance.GetPool(weapon.prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
        }
    }
}
