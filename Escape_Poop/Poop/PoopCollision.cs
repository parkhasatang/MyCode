using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopCollision : MonoBehaviour
{
    //여기에는 충돌처리를 구현해준다.
    PoopController _poopController;
    private void Awake()
    {

        _poopController = FindObjectOfType<PoopController>();
        //게임매니저를 초기화하고
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Poop"))
        {
            collision.gameObject.SetActive(false);
            _poopController.count++;
            GameManager.I.Score++;
            //게임매니저의 점수값에 점수를 더해준다 ++;
        }
        else if (collision.gameObject.CompareTag("BottomPoop"))
        {
            collision.gameObject.SetActive(false);
            _poopController.count++;
            GameManager.I.Score++;
        }
        else if (collision.gameObject.CompareTag("Pill"))
        {
            collision.gameObject.SetActive(false);
        }
        /* //알약 태그 닿으면 그냥 사라지기
         else if (collision.gameObject.CompareTag("Pill"))
         {
             collision.gameObject.SetActive(false);
         }*/

        /*//플레이어에 추가해야댈 것
        else if (collision.gameObject.CompareTag("//똥들"))//예시
        {
            // 게임오버
            //게임 오버 UI창 불러오기
            //시간 멈추기
        }*/
        // 나중에 다른 태그가 붙은 똥에 대한 충돌을 구현하고싶으면 위에랑 똑같이 tag만 수정해서 추가해주면 된다.
    }

}
