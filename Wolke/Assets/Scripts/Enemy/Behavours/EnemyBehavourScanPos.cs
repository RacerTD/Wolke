using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyBehaviourScanPos : EnemyBehaviourStep
{
    public Vector3 PosFromWhereToScan;
    public NavMeshAgent NavMeshAgent;
    public EnemyBehaviourScanPos(Vector3 posFromWhereToScan, NavMeshAgent navMeshAgent, bool interrupts, float time, bool isInterruptable) : base(interrupts, time, isInterruptable)
    {
        Debug.Log("Added scan");
        Name = "Scan";
        PosFromWhereToScan = posFromWhereToScan;
        NavMeshAgent = navMeshAgent;
    }

    public override void StartBehaviour(GameObject enemy)
    {
        NavMeshAgent.destination = PosFromWhereToScan;
        base.StartBehaviour(enemy);
    }

    public override void UpdateBehaviour(GameObject enemy)
    {
        if (NavMeshAgent.remainingDistance <= 1f)
        {
            //DO SCAN
        }
        base.UpdateBehaviour(enemy);
    }

    public override bool Ends(GameObject enemy)
    {
        return base.Ends(enemy);
    }
}