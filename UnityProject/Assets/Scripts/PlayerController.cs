using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 currentMoveInput = Vector2.zero;

    public float moveSpeed = 5f;

    public Rigidbody2D Rigidbody { get => rigid; }
    private Rigidbody2D rigid;

    public PlayerAbility fireAbility;
    public PlayerAbility dashAbility;


    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        fireAbility.Setup(this);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        fireAbility.Update();
    }

    private void Move()
    {
        rigid.MovePosition(rigid.position + currentMoveInput * Time.fixedDeltaTime * moveSpeed);
    }

    #region Input
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }
    public void PauseInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            GameManager.Main.OnPause();
        }
    }
    public void FireInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
        }
        if (context.performed)
        {
            fireAbility.OnButtonPressed();
        }
        if(context.canceled)
        {
            fireAbility.OnButtonReleased(); 
        }
    }
    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Firen
        }
    }
    #endregion
}
