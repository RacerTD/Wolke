using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBehavourWaitAtPos : EnemyBehavourStep
{
    public EnemyBehavourWaitAtPos(bool interrupts, float time, bool isInterruptable) : base(interrupts, time, isInterruptable)
    {
        Name = "Wait";
        Debug.Log("Added wait");
    }
}