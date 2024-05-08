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
    //direction이라는 변수로 만들었으면 드럽지않았다. -송지원 튜터님-
    void Move()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    void MoveRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
