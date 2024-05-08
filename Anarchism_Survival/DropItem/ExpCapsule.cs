using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpCapsule : MonoBehaviour
{
    internal float exp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (exp != 0)
            {
                GameManager.Instance.exp += exp;
            }
            else Debug.Log("����ġ�� ��� �ֽ��ϴ�.");
            gameObject.SetActive(false);
        }
    }
}
