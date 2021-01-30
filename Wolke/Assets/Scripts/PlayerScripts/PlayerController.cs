using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : AbilityController
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera controlledCamera;

    [Header("Particles")]
    [SerializeField] [Tooltip("Particles per second")] private float shootRate = 100f;
    [SerializeField] private float particleSpread = 10f;
    [SerializeField] private LayerMask particleLayerMask;
    [SerializeField] private VisualEffect singleParticleSystem;
    private InputAction.CallbackContext shootInputAction;

    [Header("View")]
    [SerializeField] private float viewSpeed = 2f;
    private InputAction.CallbackContext viewInputAction;

    protected override void Start()
    {
        GameManager.Instance.PlayerController = this;
    }

    protected override void Update()
    {
        HandleParticleShoot();
        HandleView();
        base.Update();
    }

    private void FixedUpdate()
    {
        //HandleParticleShoot();
    }

    public void HandleViewInput(InputAction.CallbackContext context)
    {
        viewInputAction = context;
    }

    private void HandleView()
    {
        if (!viewInputAction.canceled)
        {
            //controlledCamera.transform.localRotation = Quaternion.Euler()
        }
    }

    public void HandleParticleShootInput(InputAction.CallbackContext context)
    {
        shootInputAction = context;
    }

    private void HandleParticleShoot()
    {
        if ((shootInputAction.performed || shootInputAction.started) && singleParticleSystem != null)
        //if (Time.frameCount % 5 == 1)
        {

            List<Vector3> temp = new List<Vector3>();

            //Debug.Log(shootRate * Time.deltaTime);
            for (int i = 0; i < shootRate * Time.deltaTime; i++)
            {
                RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, GenerateShootDirection(), 100f, particleLayerMask);

                hits = hits.OrderBy(h => (h.point - cameraTransform.position).magnitude).ToArray();

                if (hits.Length > 0)
                {
                    temp.Add(hits[0].point);
                    //Debug.Log(hits[0].point);
                    //singleParticleSystem.SetVector3("ParticlePos", hits[0].point);
                    //singleParticleSystem.SendEvent("CreateParticle");
                }
            }

            ParticleManager.Instance.GenerateParticles(temp);
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
