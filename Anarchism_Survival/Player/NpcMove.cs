using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : EnemyMove
{
    public float minDistance = 1.0f; // 플레이어와 유지해야 할 최소 거리
    private Animator animator;
    public int npcId;

    protected override void Start()
    {
        base.Start(); // 부모 클래스의 Start 메서드 호출
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

            // 플레이어를 바라보게 하는 로직 유지
            if (direction.x > 0)
                spriteRenderer.flipX = true;
            else if (direction.x < 0)
                spriteRenderer.flipX = false;
        }
    }
}
