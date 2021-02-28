using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyBehaviourWalkToPos : EnemyBehaviourStep
{
    public Vector3 Pos = Vector3.zero;
    public NavMeshAgent NavMeshAgent;
    public EnemyBehaviourWalkToPos(Vector3 pos, NavMeshAgent navMeshAgent, bool interrupts, float time, bool isInterruptable) : base(interrupts, time, isInterruptable)
    {
        Name = "Walk to pos";
        Debug.Log("Added walk to pos");
        Pos = pos;
        NavMeshAgent = navMeshAgent;
    }

    public override void StartBehaviour(GameObject enemy)
    {
        if (enemy.GetComponent<NavMeshAgent>() != null)
        {
            enemy.GetComponent<NavMeshAgent>().destination = Pos;
        }

        base.StartBehaviour(enemy);
    }

    public override bool Ends(GameObject enemy)
    {
        if (RemainingTime <= 0f || NavMeshAgent.remainingDistance <= 1)
        {
            return true;
        }

        return base.Ends(enemy);
    }
}