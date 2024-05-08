using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : EnemyMove
{
    public float minDistance = 1.0f; // �÷��̾�� �����ؾ� �� �ּ� �Ÿ�
    private Animator animator;
    public int npcId;

    protected override void Start()
    {
        base.Start(); // �θ� Ŭ������ Start �޼��� ȣ��
        animator = GetComponent<Animator>();
        if (!GameManager.Instance.saveData.registered[npcId])
        {
            gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        if (targetPosition != null)
        {
            Vector3 direction = targetPosition.position - transform.position;
            float distance = Vector3.Distance(transform.position, targetPosition.position);
            if (distance > minDistance)
            {
                animator.SetBool("WALK", true);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("WALK", false);
            }

            // �÷��̾ �ٶ󺸰� �ϴ� ���� ����
            if (direction.x > 0)
                spriteRenderer.flipX = true;
            else if (direction.x < 0)
                spriteRenderer.flipX = false;
        }
    }
}
