using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyBehavourFollowPlayer : EnemyBehavourStep
{
    public PlayerController Player;
    public NavMeshAgent NavMeshAgent;
    public EnemyBehavourFollowPlayer(PlayerController player, NavMeshAgent navMeshAgent, float time, bool isInterruptable = false) : base(time, isInterruptable)
    {
        Player = player;
        NavMeshAgent = navMeshAgent;
    }

    public override void UpdateBehavour(GameObject enemy)
    {
        NavMeshAgent.destination = Player.transform.position;

        base.UpdateBehavour(enemy);
    }

    public override bool Ends(GameObject enemy)
    {
        if (enemy.GetComponent<EnemyController>() != null && enemy.GetComponent<EnemyController>().EnemyAlertState != EnemyAlertState.Alerted)
        {
            return true;
        }
        return base.Ends(enemy);
    }
}