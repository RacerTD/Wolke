using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyController : AbilityController
{
    #region EnemyAlertState
    [SerializeField] private EnemyAlertState enemyAlertState = EnemyAlertState.Idle;
    public EnemyAlertState EnemyAlertState
    {
        get => enemyAlertState;
        set
        {
            if (value != EnemyAlertState)
            {
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

    private void EnemyAlertStateUpdate()
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
    #endregion

    [Header("Particles")]
    [SerializeField] private float IdleFloat = 0f;
    [SerializeField] private float SusFloat = 0f;
    [SerializeField] private float AlertedFloat = 0f;
    [SerializeField] private float ColorLerpMult = 1f;
    private VisualEffect enemyParticleSystem;

    protected override void Start()
    {
        enemyParticleSystem = GetComponent<VisualEffect>();
        base.Start();
    }

    protected override void Update()
    {
        EnemyAlertStateUpdate();
        base.Update();
    }
}

public enum EnemyAlertState
{
    Idle,
    Sus,
    Alerted
}