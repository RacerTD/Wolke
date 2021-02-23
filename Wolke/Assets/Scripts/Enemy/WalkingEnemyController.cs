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

    #region behavourList
    [SerializeField] private List<EnemyBehavourStep> behavourList = new List<EnemyBehavourStep>();

    private void AddStepToQueue(EnemyBehavourStep newStep)
    {
        List<EnemyBehavourStep> temp = new List<EnemyBehavourStep>();

        if (newStep.Interrupts)
        {
            foreach (EnemyBehavourStep step in behavourList.Where(s => s.IsInterruptable))
            {
                step.Interrupt(gameObject);
            }

            foreach (EnemyBehavourStep step in behavourList.Where(s => !s.IsInterruptable))
            {
                temp.Add(step);
            }
        }
        else
        {
            foreach (EnemyBehavourStep step in behavourList)
            {
                temp.Add(step);
            }
        }

        temp.Add(newStep);

        behavourList = temp;
    }
    #endregion

    protected override void OnEnemyAlertStateChange(EnemyAlertState state)
    {
        switch (state)
        {
            case EnemyAlertState.Idle:
                break;
            case EnemyAlertState.Sus:
                break;
            case EnemyAlertState.Alerted:
                //BehavourList.Add(new EnemyBehavourFollowPlayer(GameManager.Instance.PlayerController, navMeshAgent, true, 1f, false));
                AddStepToQueue(new EnemyBehavourFollowPlayer(GameManager.Instance.PlayerController, navMeshAgent, true, 1f, false));
                break;
            default:
                break;
        }

        base.OnEnemyAlertStateChange(state);
    }

    protected override void Start()
    {
        enemyParticleSystem = GetComponentInChildren<VisualEffect>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        GenerateEnemyBehavour();
        base.Start();
    }

    protected override void Update()
    {
        ParticleColorUpdate();
        enemyParticleSystem.SetFloat("DistanceToPlayer", Vector3.Distance(GameManager.Instance.PlayerController.transform.position, transform.position));
        UpdateEnemyBehavour();
        base.Update();
    }

    private void UpdateEnemyBehavour()
    {
        if (behavourList.Count() <= 0)
        {
            GenerateEnemyBehavour();
        }

        if (behavourList.Count() > 0)
        {
            if (!behavourList.First().Started)
            {
                behavourList.First().StartBehavour(gameObject);
            }

            behavourList.First().UpdateBehavour(gameObject);

            if (behavourList.First().RemainingTime < 0)
            {
                behavourList.RemoveAt(0);
            }
        }
    }

    private void GenerateEnemyBehavour()
    {
        List<EnemyBehavourStep> temp = new List<EnemyBehavourStep>();

        switch (EnemyAlertState)
        {
            case EnemyAlertState.Idle:
                //temp.Add(new EnemyBehavourWalkToPos(enemyPath.Positions[enemyPathIndex].Position.position, navMeshAgent, false, float.MaxValue, true));
                AddStepToQueue(new EnemyBehavourWalkToPos(enemyPath.Positions[enemyPathIndex].Position.position, navMeshAgent, false, float.MaxValue, true));
                //temp.Add(new EnemyBehavourWaitAtPos(false, enemyPath.Positions[enemyPathIndex].WaitTimeAtPostion, true));
                AddStepToQueue(new EnemyBehavourWaitAtPos(false, enemyPath.Positions[enemyPathIndex].WaitTimeAtPostion, true));
                enemyPathIndex++;
                break;
            case EnemyAlertState.Sus:
                //temp.Add(new EnemyBehavourWaitAtPos(false, 3f, true));
                AddStepToQueue(new EnemyBehavourWaitAtPos(false, enemyPath.Positions[enemyPathIndex].WaitTimeAtPostion, true));
                break;
            case EnemyAlertState.Alerted:
                //temp.Add(new EnemyBehavourFollowPlayer(GameManager.Instance.PlayerController, navMeshAgent, true, 1f, false));
                AddStepToQueue(new EnemyBehavourFollowPlayer(GameManager.Instance.PlayerController, navMeshAgent, true, 1f, false));
                break;
            default:
                break;
        }

        //BehavourList.AddRange(temp);
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
}