using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/Dash", order = 0)]
[System.Serializable]
public class FireDashAbility : PlayerAbility
{
    public float speed = 2f;
    Vector2 dashDirection = Vector2.zero;

    public float dashTime = 1f;
    private float _dashTime = 0f;

    protected override void OnAbilityButtonReleased()
    {

    }

    protected override void OnAbilityStarted()
    {
        playerController.canMove = false;
        dashDirection = playerController.CurrentMoveInput;
        _dashTime = dashTime;
    }

    protected override void OnAbilityUpdate()
    {
        if (_dashTime > 0)
        {
            playerController.Rigidbody.MovePosition(playerController.Rigidbody.position + dashDirection * speed * Time.fixedDeltaTime);
            _dashTime -= Time.fixedDeltaTime;
        } else
        {
            Debug.Log($"hi");
            playerController.canMove = true;
            OnAbilityEnded();
        }
    }
}
