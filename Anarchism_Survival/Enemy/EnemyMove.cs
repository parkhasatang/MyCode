using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Transform targetPosition;
    public float speed;
    public SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // GameManager에서 플레이어 위치를 가져옴
        targetPosition = GameManager.Instance.playerPosition;
    }

    protected virtual void Update()
    {
        if (targetPosition != null)
        {
            Vector3 direction = targetPosition.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);

            // 적이 플레이어를 바라보게 하는 로직
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }
}
