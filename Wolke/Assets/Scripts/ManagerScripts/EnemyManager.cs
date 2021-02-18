using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : ManagerModule<EnemyManager>
{
    public List<EnemyController> EnemyList = new List<EnemyController>();
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
}