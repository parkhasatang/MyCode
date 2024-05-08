using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float damagePerSecond; // �ʴ� ���ط�

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

    // Ʈ���� ���� �ӹ����� ���� ���������� ȣ��Ǵ� �޼���
    void OnTriggerStay2D(Collider2D collision)
    {
        // ���� 'Enemy' �±׸� ������ ���� ��� ���ظ� �����ϴ�.
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damagePerSecond);
        }
    }
}