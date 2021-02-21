using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : ManagerModule<EnemyManager>
{
    public List<EnemyController> EnemyList = new List<EnemyController>();
    public List<AutoScanner> AutoScanner = new List<AutoScanner>();

    [Header("Variables")]
    [SerializeField] private float BroadcastDistance = 100f;

    [Header("Public Information")]
    public Vector3 newestPlayerPos = Vector3.zero;

    /// <summary>
    /// Return the current highest alertstate
    /// </summary>
    /// <returns></returns>
    public EnemyAlertState GetCurrentAlertState()
    {
        if (EnemyList.Where(x => x.EnemyAlertState == EnemyAlertState.Alerted).Count() > 0)
            return EnemyAlertState.Alerted;
        else if (EnemyList.Where(x => x.EnemyAlertState == EnemyAlertState.Sus).Count() > 0)
            return EnemyAlertState.Sus;
        else
            return EnemyAlertState.Idle;
    }

    /// <summary>
    /// Returns the nearest enemy to a position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public EnemyController GetNearestEnemy(Vector3 pos)
    {
        return EnemyList.OrderBy(x => Vector3.Distance(x.transform.position, pos)).First();
    }

    /// <summary>
    /// Broadcasts
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="enemyThatDetectedPos"></param>
    public void BradcastNewPlayerSighting(Vector3 playerPos, Vector3 enemyThatDetectedPos)
    {
        newestPlayerPos = playerPos;

        foreach (EnemyController con in EnemyList.Where(e => Vector3.Distance(e.transform.position, enemyThatDetectedPos) < BroadcastDistance))
        {
            con.GetNewPlayerIntel(playerPos);
        }
    }
}