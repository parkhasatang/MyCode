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
        // 충돌한 오브젝트의 태그가 "Enemy"인지 확인
        if (collision.CompareTag("Enemy"))
        {
            // 충돌한 오브젝트에 Enemy 컴포넌트가 있는지 확인하고, 있으면 TakeDamage 함수를 호출
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

        GameObject flameEffect = PoolManager.instance.GetPool(3); // 불바닥 프리팹
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
