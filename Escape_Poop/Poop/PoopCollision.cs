using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopCollision : MonoBehaviour
{
    //���⿡�� �浹ó���� �������ش�.
    PoopController _poopController;
    private void Awake()
    {

        _poopController = FindObjectOfType<PoopController>();
        //���ӸŴ����� �ʱ�ȭ�ϰ�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Poop"))
        {
            collision.gameObject.SetActive(false);
            _poopController.count++;
            GameManager.I.Score++;
            //���ӸŴ����� �������� ������ �����ش� ++;
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
        /* //�˾� �±� ������ �׳� �������
         else if (collision.gameObject.CompareTag("Pill"))
         {
             collision.gameObject.SetActive(false);
         }*/

        /*//�÷��̾ �߰��ؾߴ� ��
        else if (collision.gameObject.CompareTag("//�˵�"))//����
        {
            // ���ӿ���
            //���� ���� UIâ �ҷ�����
            //�ð� ���߱�
        }*/
        // ���߿� �ٸ� �±װ� ���� �˿� ���� �浹�� �����ϰ������ ������ �Ȱ��� tag�� �����ؼ� �߰����ָ� �ȴ�.
    }

}
