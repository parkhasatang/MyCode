using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float damagePerSecond; // 초당 피해량

    public void Init(float damage)
    {
        damagePerSecond = damage;
    }

    private void OnEnable()
    {
        StartCoroutine(ActiveFalse());
    }

    IEnumerator ActiveFalse()
    {
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
    }

    // 트리거 내에 머무르는 동안 지속적으로 호출되는 메서드
    void OnTriggerStay2D(Collider2D collision)
    {
        // 적이 'Enemy' 태그를 가지고 있을 경우 피해를 입힙니다.
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damagePerSecond);
        }
    }
}