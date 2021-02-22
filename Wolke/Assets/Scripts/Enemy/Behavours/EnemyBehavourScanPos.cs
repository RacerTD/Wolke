using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyBehavourScanPos : EnemyBehavourStep
{
    public Vector3 PosFromWhereToScan;
    public NavMeshAgent NavMeshAgent;
    public EnemyBehavourScanPos(Vector3 posFromWhereToScan, NavMeshAgent navMeshAgent, float time, bool isInterruptable = false) : base(time, isInterruptable)
    {
        PosFromWhereToScan = posFromWhereToScan;
        NavMeshAgent = navMeshAgent;
    }

    public override void StartBehavour(GameObject enemy)
    {
        NavMeshAgent.destination = PosFromWhereToScan;
        base.StartBehavour(enemy);
    }

    public override void UpdateBehavour(GameObject enemy)
    {
        if (NavMeshAgent.remainingDistance <= 1f)
        {
            //DO SCAN
        }
        base.UpdateBehavour(enemy);
    }

    public override bool Ends(GameObject enemy)
    {
        return base.Ends(enemy);
    }
}