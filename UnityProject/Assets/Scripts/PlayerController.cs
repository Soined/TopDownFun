using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    Vector2 currentMoveInput = Vector2.zero;
    public Vector2 CurrentMoveInput { get => currentMoveInput; }

    public float moveSpeed = 5f;

    public Rigidbody2D Rigidbody { get => rigid; }
    private Rigidbody2D rigid;

    public PlayerAbility fireAbility;
    public PlayerAbility dashAbility;

    public bool canMove = true;


    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        fireAbility.Setup(this);
        dashAbility.Setup(this);
    }

    private void FixedUpdate()
    {
        Move();

        fireAbility.Update();
        dashAbility.Update();
    }

    private void Update()
    {

    }

    private void Move()
    {
        if (!canMove) return;

        rigid.MovePosition(rigid.position + currentMoveInput * Time.fixedDeltaTime * moveSpeed);
    }

    #region Input
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }
    public void PauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Main.OnPause();
        }
    }
    public void FireInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fireAbility.OnButtonPressed();
        }
        if (context.canceled)
        {
            fireAbility.OnButtonReleased();
        }
    }
    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashAbility.OnButtonPressed();
        }
        if (context.canceled)
        {
            dashAbility.OnButtonReleased();
        }
    }

    public void TakeDamage(int damage)
    {
        //Schaden bekommen
    }
    #endregion
}
