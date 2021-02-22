using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBehavourWaitAtPos : EnemyBehavourStep
{
    public EnemyBehavourWaitAtPos(float time, bool isInterruptable = false) : base(time, isInterruptable)
    {

    }
}