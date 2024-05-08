using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float maxDragDistance = 100.0f;

    private Vector2 startPosition;
    private Vector2 direction;
    private bool isDragging = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 마우스 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 dragVector = mousePosition - startPosition;
            float dragDistance = dragVector.magnitude;  

            direction = dragVector.normalized;
            float speedFactor = Mathf.Clamp(dragDistance / maxDragDistance, 0, 1);
            MovePlayer(direction, speedFactor);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 추가적으로, 모바일 장치용 터치 입력 처리
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 endPosition = touch.position;
                        Vector2 dragVector = endPosition - startPosition;
                        float dragDistance = dragVector.magnitude;

                        direction = dragVector.normalized;
                        float speedFactor = Mathf.Clamp(dragDistance / maxDragDistance, 0, 1);
                        MovePlayer(direction, speedFactor);
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }

        if (direction.x > 0) spriteRenderer.flipX = false;
        else if (direction.x < 0) spriteRenderer.flipX = true;
        animator.SetBool("WALK", isDragging);
    }

    private void MovePlayer(Vector2 dir, float speedFactor)
    {
        Vector2 moveVector = dir * moveSpeed * speedFactor * Time.deltaTime;
        transform.Translate(moveVector);
    }
}
