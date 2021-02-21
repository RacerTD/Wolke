using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class WalkingEnemyController : EnemyController
{
    [Header("Particles")]
    private float IdleFloat = 0f;
    private float SusFloat = 0f;
    private float AlertedFloat = 0f;
    [SerializeField] private float ColorLerpMult = 1f;
    private VisualEffect enemyParticleSystem;

    [Header("AI")]
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
    private WalkingEnemyBehavior currentBehavior = WalkingEnemyBehavior.WaitingAtPos;
    public WalkingEnemyBehavior CurrentBehavior
    {
        get => currentBehavior;
        set
        {
            if (value != currentBehavior)
            {
                currentBehavior = value;
                StartNewWalkingEnemyBehavior(value);
                timeInCurrentBehavour = 0f;
            }
        }
    }
    [SerializeField] private float timeInCurrentBehavour = 0f;

    private void StartNewWalkingEnemyBehavior(WalkingEnemyBehavior value)
    {
        switch (value)
        {
            case WalkingEnemyBehavior.WalkingAround:
                break;
            case WalkingEnemyBehavior.WaitingAtPos:
                break;
            case WalkingEnemyBehavior.ScanneingPos:
                break;
            case WalkingEnemyBehavior.FollowingPlayer:
                break;
        }
    }

    protected override void Start()
    {
        enemyParticleSystem = GetComponentInChildren<VisualEffect>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        base.Start();
    }

    protected override void Update()
    {
        ParticleColorUpdate();
        enemyParticleSystem.SetFloat("DistanceToPlayer", Vector3.Distance(GameManager.Instance.PlayerController.transform.position, transform.position));
        UpdateEnemyWalk();
        base.Update();
    }

    private void UpdateWalkingEnemyBehavior()
    {
        switch (currentBehavior)
        {
            case WalkingEnemyBehavior.WalkingAround:
                break;
            case WalkingEnemyBehavior.WaitingAtPos:
                break;
            case WalkingEnemyBehavior.ScanneingPos:
                break;
            case WalkingEnemyBehavior.FollowingPlayer:
                break;
        }
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
}

public enum WalkingEnemyBehavior
{
    WalkingAround,
    WaitingAtPos,
    ScanneingPos,
    FollowingPlayer
}