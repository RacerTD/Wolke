using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : AbilityController
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera controlledCamera;
    [SerializeField] [Tooltip("Particles per second")] private float shootRate = 100f;
    private InputAction.CallbackContext shootInputAction;
    [SerializeField] private float particleSpread = 10f;
    [SerializeField] private LayerMask particleLayerMask;
    [SerializeField] private VisualEffect singleParticleSystem;

    protected override void Start()
    {
        GameManager.Instance.PlayerController = this;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        HandleParticleShoot();
    }

    public void HandleViewInput(InputAction.CallbackContext context)
    {

    }

    public void HandleParticleShootInput(InputAction.CallbackContext context)
    {
        shootInputAction = context;
    }

    private void HandleParticleShoot()
    {
        if ((shootInputAction.performed || shootInputAction.started) && singleParticleSystem != null)
        {
            //Debug.Log(shootRate * Time.deltaTime);
            for (int i = 0; i < shootRate * Time.fixedDeltaTime; i++)
            {
                RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, GenerateShootDirection(), 100f, particleLayerMask);

                if (hits.Length > 0)
                {
                    //Debug.Log(hits[0].point);
                    singleParticleSystem.SetVector3("ParticlePos", hits[0].point);
                    singleParticleSystem.SendEvent("CreateParticle");
                }
            }
        }
    }

    private Vector3 GenerateShootDirection()
    {
        return ((cameraTransform.position + cameraTransform.forward * 1000).normalized) + new Vector3(Random.Range(particleSpread, -particleSpread) / 100, Random.Range(particleSpread, -particleSpread) / 100, Random.Range(particleSpread, -particleSpread) / 100);
    }

    /// <summary>
    /// Happens when the input device changes
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceChanged(PlayerInput input)
    {
        Debug.Log($"Input device changed");
        foreach (PlayerAbility playerAbility in Abilities)
        {
            playerAbility.OnInputDeviceChanged(input);
        }
    }

    /// <summary>
    /// Happens when the current input device disconnects
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceLost(PlayerInput input)
    {
        Debug.Log($"Input device lost");
        foreach (PlayerAbility playerAbility in Abilities)
        {
            playerAbility.OnInputDeviceLost(input);
        }
    }

    /// <summary>
    /// Happens when the current input device is reconnected
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceReconnected(PlayerInput input)
    {
        Debug.Log($"Input device reconnected");
        foreach (PlayerAbility playerAbility in Abilities)
        {
            playerAbility.OnInputDeviceReconnected(input);
        }
    }
}
