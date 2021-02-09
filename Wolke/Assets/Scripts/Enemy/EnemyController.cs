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
    [SerializeField] private float timeInAltertState = 0f;
    [SerializeField] private EnemyAlertState enemyAlertState = EnemyAlertState.Idle;
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

    private void OnEnemyAlertStateChange(EnemyAlertState state)
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

    [Header("Particles")]
    private float IdleFloat = 0f;
    private float SusFloat = 0f;
    private float AlertedFloat = 0f;
    [SerializeField] private float ColorLerpMult = 1f;
    private VisualEffect enemyParticleSystem;

    [Header("AI Relevant Stuff")]
    [SerializeField] private PlayerData playerData = new PlayerData();
    [SerializeField] private float enemyDataUpdateIntervall = 0.5f;
    private float timeSinceLastDataUpdate = 0f;
    [SerializeField] private float maxSightingSistance = 10f;
    [SerializeField] private Transform viewPoint;
    [SerializeField] private LayerMask forEnemyVisibleMask = new LayerMask();
    [SerializeField] private EnemyPath enemyPath;
    private int _enemyPathIndex = 0;
    private int enemyPathIndex
    {
        get => _enemyPathIndex;
        set
        {
            if (value > enemyPath.Positions.Count() - 1)
            {
                _enemyPathIndex = 0;
            }
            else
            {
                _enemyPathIndex = value;
            }
        }
    }
    private NavMeshAgent navMeshAgent;

    protected override void Start()
    {
        enemyParticleSystem = GetComponentInChildren<VisualEffect>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (!GameManager.Instance.EnemyList.Contains(this))
        {
            GameManager.Instance.EnemyList.Add(this);
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

        ParticleColorUpdate();

        enemyParticleSystem.SetFloat("DistanceToPlayer", Vector3.Distance(GameManager.Instance.PlayerController.transform.position, transform.position));

        UpdateEnemyWalk();

        timeInAltertState += Time.deltaTime;

        base.Update();
    }

    private void ParticleColorUpdate()
    {
        switch (EnemyAlertState)
        {
            case EnemyAlertState.Idle:
                IdleFloat = Mathf.Lerp(IdleFloat, 1, ColorLerpMult * Time.deltaTime);
                SusFloat = Mathf.Lerp(SusFloat, 0, ColorLerpMult * Time.deltaTime);
                AlertedFloat = Mathf.Lerp(AlertedFloat, 0, ColorLerpMult * Time.deltaTime);
                break;
            case EnemyAlertState.Sus:
                IdleFloat = Mathf.Lerp(IdleFloat, 0, ColorLerpMult * Time.deltaTime);
                SusFloat = Mathf.Lerp(SusFloat, 1, ColorLerpMult * Time.deltaTime);
                AlertedFloat = Mathf.Lerp(AlertedFloat, 0, ColorLerpMult * Time.deltaTime);
                break;
            case EnemyAlertState.Alerted:
                IdleFloat = Mathf.Lerp(IdleFloat, 0, ColorLerpMult * Time.deltaTime);
                SusFloat = Mathf.Lerp(SusFloat, 0, ColorLerpMult * Time.deltaTime);
                AlertedFloat = Mathf.Lerp(AlertedFloat, 1, ColorLerpMult * Time.deltaTime);
                break;
            default:
                break;
        }

        enemyParticleSystem.SetFloat("IdleColorMult", IdleFloat);
        enemyParticleSystem.SetFloat("SusColorMult", SusFloat);
        enemyParticleSystem.SetFloat("AlertedColorMult", AlertedFloat);
    }

    private void CalculateEnemyAlertState()
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
                }
                break;
            default:
                break;
        }
    }

    private void UpdateEnemyWalk()
    {
        switch (EnemyAlertState)
        {
            case EnemyAlertState.Idle:
                if (navMeshAgent.remainingDistance <= 1f && enemyPath != null)
                {
                    enemyPathIndex++;
                    navMeshAgent.destination = enemyPath.Positions[enemyPathIndex].Position.position;
                }
                break;
            case EnemyAlertState.Sus:
                navMeshAgent.destination = transform.position;
                break;
            case EnemyAlertState.Alerted:
                navMeshAgent.destination = GameManager.Instance.PlayerController.transform.position;
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
            data.TimeSinceLastSighting = 0f;
        else
            data.TimeSinceLastSighting += Time.deltaTime;
    }

    private bool PlayerVisibleForEnemy(ref PlayerData data)
    {
        return data.DistanceToPlayer < maxSightingSistance && data.HasDirectSight && data.PlayerInDetectionCollider;
    }

    public void SetDataDetectionCollider(bool value)
    {
        playerData.PlayerInDetectionCollider = value;
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
}

public enum EnemyAlertState
{
    Idle,
    Sus,
    Alerted
}