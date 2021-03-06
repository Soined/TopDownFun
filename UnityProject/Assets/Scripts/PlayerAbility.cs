using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerAbility : ScriptableObject
{
    protected PlayerController playerController;

    [SerializeField]
    protected float cooldown = 1f;
    private float _cooldown = 0f;

    public bool IsOnCooldown { get => _cooldown >= 0; }

    public bool IsActive { get => isActive; }
    private bool isActive = false;

    public void Setup(PlayerController controller)
    {
        playerController = controller;
    }

    public void Update()
    {
        if(IsOnCooldown)
        {
            _cooldown -= Time.fixedDeltaTime;
        }
        if(isActive)
        OnAbilityUpdate();
    }

    public void OnButtonPressed()
    {
        if (IsOnCooldown || isActive)
        {
            return;
        }

        isActive = true;
        OnAbilityStarted(); 
    }
    public void OnButtonReleased()
    {
        OnAbilityButtonReleased();
    }
    /// <summary>
    /// Is called when the Button is pressed and the Ability is not on Cooldown
    /// </summary>
    protected abstract void OnAbilityStarted();
    /// <summary>
    /// Is called when the player lets go of the Button
    /// </summary>
    protected abstract void OnAbilityButtonReleased();
    /// <summary>
    /// Is called every Frame the ability is active
    /// </summary>
    protected abstract void OnAbilityUpdate();

    /// <summary>
    /// Has to be called when the Ability is finished
    /// </summary>
    protected void OnAbilityEnded()
    {
        _cooldown = cooldown;
        isActive = false;
    }


}
