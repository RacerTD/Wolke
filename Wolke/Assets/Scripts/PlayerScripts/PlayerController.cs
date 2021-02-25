using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : AbilityController
{
    [Header("General")]
    [SerializeField] private bool isDead = false;
    public bool IsDead
    {
        get => isDead;
        set
        {
            if (value != isDead)
            {
                isDead = value;

                if (!isDead)
                {
                    timeSinceDeath = 0f;
                }
            }
        }
    }
    private float timeSinceDeath = 0f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera controlledCamera;

    [Header("Particles")]
    [SerializeField] [Tooltip("Particles per second")] private float shootRate = 100f;
    public float ShootRate
    {
        get => shootRate;
        set
        {
            ParticleManager.Instance.ParticlesPerSecond = value;
            shootRate = value;
        }
    }
    [SerializeField] private float particleSpread = 10f;
    public float ParticleSpread
    {
        get => particleSpread;
        set
        {
            ParticleManager.Instance.ParticleSpread = value;
            particleSpread = value;
        }
    }

    [Header("View")]
    [SerializeField] private float viewSpeed = 2f;
    private float rotationX = 0f;
    private InputAction.CallbackContext viewInputAction;
    private Vector2 viewInputVector = Vector2.zero;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool isMoving;
    private Vector2 moveInputVector = Vector2.zero;
    private Rigidbody physicsbody;
    public Vector3 MoveVector = Vector3.zero;
    public Vector3 MoveShouldVector = Vector3.zero;
    [SerializeField] private float accelerationSpeed = 5f;


    protected override void Start()
    {
        physicsbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        ParticleManager.Instance.ParticlesPerSecond = shootRate;
    }

    protected override void Update()
    {
        HandleView();

        if (IsDead)
        {
            timeSinceDeath += Time.deltaTime;
            ParticleManager.Instance.PlayerIsDead(timeSinceDeath);
        }

        base.Update();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    #region Move
    public void HandleMoveInput(InputAction.CallbackContext context)
    {
        isMoving = !context.canceled;
        moveInputVector = context.ReadValue<Vector2>();
    }

    private void HandleMove()
    {
        if (!IsDead)
        {
            if (isMoving)
                MoveShouldVector = new Vector3(moveInputVector.x * moveSpeed, 0, moveInputVector.y * moveSpeed);
            else
                MoveShouldVector = Vector3.zero;

            MoveVector = Vector3.Lerp(MoveVector, MoveShouldVector, accelerationSpeed * Time.fixedDeltaTime);

            transform.Translate(Vector3.RotateTowards(transform.position, MoveVector * Time.fixedDeltaTime, float.MaxValue, float.MaxValue));

            //physicsbody.MovePosition(transform.position + MoveVector);
        }
    }
    #endregion

    #region View
    public void HandleViewInput(InputAction.CallbackContext context)
    {
        viewInputAction = context;
        viewInputVector = context.ReadValue<Vector2>();
    }

    private void HandleView()
    {
        if (!IsDead)
        {
            if (!viewInputAction.canceled)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, viewInputVector.x * viewSpeed * Time.deltaTime + transform.localRotation.eulerAngles.y, 0));
                rotationX += -viewInputVector.y * viewSpeed * Time.deltaTime;
                rotationX = Mathf.Clamp(rotationX, -85, 85);
                cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            }

            Debug.DrawRay(controlledCamera.transform.position, controlledCamera.transform.forward, Color.red);
        }
    }
    #endregion

    #region Particles
    public void HandleParticleShootInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            ParticleManager.Instance.SetParticleGeneratorState(false);
        }
        else
        {
            ParticleManager.Instance.SetParticleGeneratorState(true);
        }
    }
    #endregion

    #region TestInputs
    public void HandleTestInput(InputAction.CallbackContext context)
    {
        IsDead = true;
    }

    public void HandleTestInput2(InputAction.CallbackContext context)
    {
        IsDead = false;
        GameManager.Instance.ResetPlayerToSpawn();
        ParticleManager.Instance.PlayerIsDead(0f);
    }
    #endregion

    #region Input Device things
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
    #endregion
}
