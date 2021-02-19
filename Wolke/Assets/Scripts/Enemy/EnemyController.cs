using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : AbilityController
{
    #region EnemyAlertState
    [SerializeField] protected float timeInAltertState = 0f;
    [SerializeField] protected EnemyAlertState enemyAlertState = EnemyAlertState.Idle;
    public EnemyAlertState EnemyAlertState
    {
        get => enemyAlertState;
        set
        {
            if (value != EnemyAlertState)
            {
                timeInAltertState = 0f;
                OnEnemyAlertStateChange(value);
                enemyAlertState = value;
            }
        }
    }

    protected virtual void OnEnemyAlertStateChange(EnemyAlertState state)
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
    }
    #endregion

    [Header("AI Relevant Stuff")]
    [SerializeField] protected PlayerData playerData = new PlayerData();
    [SerializeField] private float enemyDataUpdateIntervall = 0.5f;
    private float timeSinceLastDataUpdate = 0f;
    [SerializeField] private float maxSightingSistance = 10f;
    [SerializeField] protected Transform viewPoint;
    [SerializeField] private LayerMask forEnemyVisibleMask = new LayerMask();

    protected override void Start()
    {
        if (!EnemyManager.Instance.EnemyList.Contains(this))
        {
            EnemyManager.Instance.EnemyList.Add(this);
        }

        base.Start();
    }

    protected override void Update()
    {
        timeSinceLastDataUpdate += Time.deltaTime;

        if (timeSinceLastDataUpdate >= enemyDataUpdateIntervall)
        {
            UpdatePlayerData(ref playerData);
            timeSinceLastDataUpdate = 0f;
        }

        UpdateFramePerfectPlayerData(ref playerData);

        CalculateEnemyAlertState();

        timeInAltertState += Time.deltaTime;

        base.Update();
    }

    protected virtual void CalculateEnemyAlertState()
    {
        switch (EnemyAlertState)
        {
            case EnemyAlertState.Idle:
                if (PlayerVisibleForEnemy(ref playerData))
                {
                    EnemyAlertState = EnemyAlertState.Sus;
                }
                break;
            case EnemyAlertState.Sus:
                if (PlayerVisibleForEnemy(ref playerData) && timeInAltertState >= 5f && playerData.TimeSinceLastSighting <= 5f)
                {
                    EnemyAlertState = EnemyAlertState.Alerted;
                }
                else if (playerData.TimeSinceLastSighting >= 10f && !PlayerVisibleForEnemy(ref playerData))
                {
                    EnemyAlertState = EnemyAlertState.Idle;
                }
                break;
            case EnemyAlertState.Alerted:
                if (playerData.TimeSinceLastSighting >= 5f)
                {
                    EnemyAlertState = EnemyAlertState.Sus;
                    EnemyManager.Instance.BradcastNewPlayerSighting(playerData.LastSeenPlayerPos, transform.position);
                }
                break;
            default:
                break;
        }
    }

    private void UpdatePlayerData(ref PlayerData data)
    {
        data.DistanceToPlayer = Vector3.Distance(GameManager.Instance.PlayerController.transform.position, transform.position);
        data.ViewAngleToPlayer = Vector3.Angle((GameManager.Instance.PlayerController.transform.position) - viewPoint.position, viewPoint.forward);
        data.HasDirectSight = HasDirectSight();
    }

    private bool HasDirectSight()
    {
        Debug.DrawRay(viewPoint.position, (GameManager.Instance.PlayerController.transform.position - viewPoint.position).normalized * playerData.DistanceToPlayer, Color.green, enemyDataUpdateIntervall);

        RaycastHit[] hits = Physics.RaycastAll(viewPoint.position, GameManager.Instance.PlayerController.transform.position - viewPoint.position, forEnemyVisibleMask);
        hits = hits.OrderBy(h => (h.point + viewPoint.position).magnitude).ToArray();

        return hits.OrderBy(h => (h.point - viewPoint.position).magnitude).ToArray().Select(hit => hit.collider.GetComponent<PlayerController>() != null).FirstOrDefault();
    }

    private void UpdateFramePerfectPlayerData(ref PlayerData data)
    {
        if (PlayerVisibleForEnemy(ref data))
        {
            data.TimeSinceLastSighting = 0f;
            data.LastSeenPlayerPos = GameManager.Instance.PlayerController.transform.position;
        }
        else
        {
            data.TimeSinceLastSighting += Time.deltaTime;
        }
    }

    private bool PlayerVisibleForEnemy(ref PlayerData data)
    {
        return data.DistanceToPlayer < maxSightingSistance && data.HasDirectSight && data.PlayerInDetectionCollider;
    }

    public void SetDataDetectionCollider(bool value)
    {
        playerData.PlayerInDetectionCollider = value;
    }

    /// <summary>
    /// Gives new intel to the enemy
    /// </summary>
    /// <param name="pos"></param>
    public virtual void GetNewPlayerIntel(Vector3 pos)
    {
        playerData.lastPublicPlayerPos = pos;
        playerData.TimeSinceLastPublicPos = 0f;
    }
}

[System.Serializable]
public struct PlayerData
{
    public float DistanceToPlayer;
    public float ViewAngleToPlayer;
    public bool PlayerInDetectionCollider;
    public bool HasDirectSight;
    public float TimeSinceLastSighting;
    public Vector3 LastSeenPlayerPos;
    public float TimeSinceLastPublicPos;
    public Vector3 lastPublicPlayerPos;
}

public enum EnemyAlertState
{
    Idle,
    Sus,
    Alerted
}