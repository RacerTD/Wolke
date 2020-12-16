using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Ability Data")]
    public float AbilityDuration = 0f;
    public float AbilityCoolDown = 0f;
    public float TimeSinceLastUse = 0f;
    public float TimeAbilityBlocked = 0f;
    [SerializeField] protected int TimesUsed = 0;
    protected AbilityController controller;

    protected virtual void Start()
    {
        controller = GetComponent<AbilityController>();
    }

    /// <summary>
    /// Trys to start the ability
    /// </summary>
    public virtual void TryAbiltiy()
    {

    }

    /// <summary>
    /// Happens every frame
    /// </summary>
    public virtual void PermanentUpdate()
    {
        if (AbilityActive() && TimeSinceLastUse + Time.deltaTime > AbilityDuration)
        {
            AbilityEnd();
            CoolDownStart();
        }

        if (AbilityOnCoolDown() && TimeSinceLastUse + Time.deltaTime > AbilityDuration + AbilityCoolDown)
        {
            CoolDownEnd();
        }

        TimeSinceLastUse += Time.deltaTime;
        TimeAbilityBlocked -= Time.deltaTime;
    }

    /// <summary>
    /// happens at the start ot the ability
    /// </summary>
    public virtual void AbilityStart()
    {
        TimesUsed++;
        TimeSinceLastUse = 0f;
    }

    /// <summary>
    /// Happens during the entire ability
    /// </summary>
    public virtual void AbilityUpdate()
    {

    }

    /// <summary>
    /// Happens at the end of the ability
    /// </summary>
    public virtual void AbilityEnd()
    {

    }

    /// <summary>
    /// Happens at the start of the cooldown
    /// </summary>
    public virtual void CoolDownStart()
    {

    }

    /// <summary>
    /// Happens during the entire cooldown
    /// </summary>
    public virtual void CoolDownUpdate()
    {

    }

    /// <summary>
    /// Happens at the end ot the cooldown
    /// </summary>
    public virtual void CoolDownEnd()
    {

    }

    /// <summary>
    /// Returns if the abiltiy is on cooldown
    /// </summary>
    public bool AbilityOnCoolDown()
    {
        return TimeSinceLastUse > AbilityDuration && TimeSinceLastUse < AbilityDuration + AbilityCoolDown;
    }

    /// <summary>
    /// Returns if the ability is currently active
    /// </summary>
    public bool AbilityActive()
    {
        return TimeSinceLastUse < AbilityDuration;
    }

    /// <summary>
    /// Returns if the abiltiy is currently blocked
    /// </summary>
    public bool AbilityBlocked()
    {
        return TimeAbilityBlocked > 0f;
    }

    /// <summary>
    /// Returns if the ability can be started
    /// </summary>
    protected virtual bool AbilityCanBeStarted()
    {
        return true;
    }
}
