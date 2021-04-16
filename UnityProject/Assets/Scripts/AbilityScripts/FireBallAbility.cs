using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ShootAbility", menuName = "Abilities/Shoot", order = 0)]
[System.Serializable]
public class FireBallAbility : PlayerAbility
{
    [SerializeField]
    protected int damage = 1;
    [SerializeField]
    protected float missileSpeed = 5;
    [SerializeField]
    protected float missileScale = 1;
    [SerializeField]
    protected Missile missile;

    protected override void OnAbilityStarted()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePosition - playerController.Rigidbody.position;

        Instantiate(missile, playerController.transform.position, Quaternion.identity)
            .Setup(damage, missileSpeed, direction, missileScale);

        OnAbilityEnded();
    }


    protected override void OnAbilityButtonReleased() { }
    protected override void OnAbilityUpdate() { }
}
