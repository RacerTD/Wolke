using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBehavourStep
{
    public string Name = "Empty";
    public bool Interrupts = false;
    public float RemainingTime = 0f;
    public bool IsInterruptable = false;
    public bool Started = false;

    public EnemyBehavourStep(bool interrupts, float remainingTime, bool isInterruptable)
    {
        Interrupts = interrupts;
        RemainingTime = remainingTime;
        IsInterruptable = isInterruptable;
    }

    /// <summary>
    /// Happens at the start of the behavour
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void StartBehavour(GameObject enemy)
    {
        Started = true;
    }

    /// <summary>
    /// Happens during the entirety of the behavour
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void UpdateBehavour(GameObject enemy)
    {
        RemainingTime -= Time.deltaTime;

        if (Ends(enemy))
        {
            EndBehavour(enemy);
        }
    }

    /// <summary>
    /// Happens once at the end of the behavour
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void EndBehavour(GameObject enemy)
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
            EndBehavour(enemy);
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