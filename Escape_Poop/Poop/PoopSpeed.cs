using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSpeed : MonoBehaviour
{

    public float speed = 5f;


    void Update()
    {
        if (CompareTag("Poop") || CompareTag("Pill"))
        {
            Move();
        }
        else if (CompareTag("BottomPoop"))
        {
            MoveRight();
        }
    }
    //direction�̶�� ������ ��������� �巴���ʾҴ�. -������ Ʃ�ʹ�-
    void Move()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    void MoveRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
