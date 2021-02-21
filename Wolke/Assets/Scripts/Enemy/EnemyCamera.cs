using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyCamera : EnemyController
{
    private AutoScanner autoScanner;

    protected override void Start()
    {
        if (GetComponentInChildren<AutoScanner>() != null)
        {
            autoScanner = GetComponentInChildren<AutoScanner>();
        }
        base.Start();
    }

    protected override void Update()
    {
        viewPoint.rotation = Quaternion.Euler(autoScanner.CurrentBaseRotation);
        base.Update();
    }

    protected override void OnEnemyAlertStateChange(EnemyAlertState state)
    {
        switch (state)
        {
            case EnemyAlertState.Idle:
                break;
            case EnemyAlertState.Sus:
                break;
            case EnemyAlertState.Alerted:
                break;
            default:
                break;
        }

        base.OnEnemyAlertStateChange(state);
    }
}
