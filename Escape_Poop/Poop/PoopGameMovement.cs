using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoopGameMovement : MonoBehaviour
{
    private CharacterController _controller;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementDirection = Vector2.zero;
    public float moveSpeed;
    public float jumpPower;
    bool isJumpAvailable;
    public Animator anim;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _controller.OnMoveEvent += Move;
        _controller.OnJumpEvent += Jump;
    }

    private void FixedUpdate()
    {
        ApplyMovement(_movementDirection);
        
    }

    private void Move(Vector2 direction)
    {
        _movementDirection = direction;
        anim.SetBool("IsRunning", true);
    }

    private void Jump(bool temp)
    {
        if (!isJumpAvailable)
        {
            isJumpAvailable = true;
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
        }
        else
        {
            return;
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bottom"))
        {
            isJumpAvailable = false;
        }
    }

    public void ApplyMovement(Vector2 direction)
    {
        

        if(direction.x > 0)
        {
            _rigidbody.velocity = new Vector2(moveSpeed, _rigidbody.velocity.y);
            _spriteRenderer.flipX = false;
        }
        else if(direction.x < 0)
        {
            _rigidbody.velocity = new Vector2((-1*moveSpeed), _rigidbody.velocity.y);
            _spriteRenderer.flipX = true;
        }
        else
        {
            _rigidbody.velocity =  new Vector2(0, _rigidbody.velocity.y);
            anim.SetBool("IsRunning", false);
        }
    }
}
