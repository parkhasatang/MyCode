using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public float damage;

    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnEnable()
    {
        if (gameObject.CompareTag("FireBomb"))
        {
            FireBombExplode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹�� ������Ʈ�� �±װ� "Enemy"���� Ȯ��
        if (collision.CompareTag("Enemy"))
        {
            // �浹�� ������Ʈ�� Enemy ������Ʈ�� �ִ��� Ȯ���ϰ�, ������ TakeDamage �Լ��� ȣ��
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            TypeByBullet(gameObject);
        }
    }


    public void RiflePenetrate()
    {
        if (gameObject.CompareTag("Rifle"))
        {
            Invoke("DeactivateProjectile", 1f);
        }
    }

    public void FireBombExplode()
    {
        StartCoroutine(ExplodeAfterDelay(transform.position));
    }

    private IEnumerator ExplodeAfterDelay(Vector3 position)
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        GameObject flameEffect = PoolManager.instance.GetPool(3); // �ҹٴ� ������
        flameEffect.transform.position = position;
        gameObject.SetActive(false);
    }

    public void TypeByBullet(GameObject obj)
    {
        switch (obj.tag)
        {
            case "Torch":
                break;
            case "Rifle":
                RiflePenetrate();
                break;
            default:
                obj.SetActive(false);
                break;
        }
    }

    void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }
}
