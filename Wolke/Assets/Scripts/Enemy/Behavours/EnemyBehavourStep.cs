using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBehaviourStep
{
    [HideInInspector] public string Name = "Empty";
    public bool Interrupts = false;
    public float RemainingTime = 0f;
    public bool IsInterruptable = false;
    public bool Started = false;

    public EnemyBehaviourStep(bool interrupts, float remainingTime, bool isInterruptable)
    {
        Interrupts = interrupts;
        RemainingTime = remainingTime;
        IsInterruptable = isInterruptable;
    }

    /// <summary>
    /// Happens at the start of the Behavior
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void StartBehaviour(GameObject enemy)
    {
        Started = true;
    }

    /// <summary>
    /// Happens during the entirety of the Behavior
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void UpdateBehaviour(GameObject enemy)
    {
        RemainingTime -= Time.deltaTime;

        if (Ends(enemy))
        {
            EndBehaviour(enemy);
        }
    }

    /// <summary>
    /// Happens once at the end of the Behavior
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void EndBehaviour(GameObject enemy)
    {
        RemainingTime = float.MinValue;
    }

    /// <summary>
    /// Happens when an interruption is on the way
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void Interrupt(GameObject enemy)
    {
        if (IsInterruptable)
        {
            EndBehaviour(enemy);
        }
    }

    /// <summary>
    /// Checks if the behavour is ending
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public virtual bool Ends(GameObject enemy)
    {
        return false;
    }
}