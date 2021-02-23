using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : ManagerModule<ParticleManager>
{
    [Header("Debug")]
    [SerializeField] private GameObject particleDebug;
    [SerializeField] private bool enableDebug = true;

    [Header("Particles")]
    [SerializeField] private List<ParticleStruct> particlePool = new List<ParticleStruct>();
    [SerializeField] private List<ParticlePrefabStruct> particlePrefabs = new List<ParticlePrefabStruct>();
    private int particlePoolIndex = 0;
    public int ParticlePoolIndex
    {
        get => particlePoolIndex;
        set
        {
            if (value > particlePool.Count() - 1)
            {
                particlePoolIndex = 0;
            }
            else
            {
                particlePoolIndex = value;
            }
        }
    }
    [SerializeField] private int maxParticlesInSystem;
    [SerializeField] private int totalSpawnedParticles = 0;
    [SerializeField] [Tooltip("Everything the particles can hit")] private LayerMask particleLayerMask;
    [SerializeField] [Tooltip("Everything particles can not stick to")] private LayerMask particleBlockMask;

    #region PlayerParticles
    private float particlesPerSecond = 1000f;
    public float ParticlesPerSecond
    {
        get => particlesPerSecond;
        set
        {
            particlesPerSecond = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }
    private float particleSpread = 20f;
    public float ParticleSpread
    {
        get => particleSpread;
        set
        {
            particleSpread = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }
    #endregion

    [Header("Gradient Stuff")]
    [SerializeField] private int lerpTime = 1;
    [SerializeField] private Gradient idleGradient;
    [SerializeField] private Gradient susGradient;
    [SerializeField] private Gradient alertedGradient;
    private Gradient currentGradient = new Gradient();

    private Transform particleParent;
    private bool particleGeneratorActive = false;
    public void SetParticleGeneratorState(bool value) => particleGeneratorActive = value;
    private Camera cam;

    public void Start()
    {
        cam = Camera.main;
        FillParticlePool(1, ParticleSystemName.Player);
    }

    public void Update()
    {
        UpdateParticleSystemPos();

        //PlayerParticle Stuff
        if (particleGeneratorActive && !GameManager.Instance.PlayerController.IsDead)
        {
            AddParticlesToQueue(GenerateParticlePositions(), ParticleSystemName.Player);
        }

        ExecuteParticleQueue();

        UpdateLivingParticleCounter();

        UpdateMainParticleGradient();
    }

    #region Create Particles
    /// <summary>
    /// Adds Particles to the correct particle queue
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="name"></param>
    public void AddParticlesToQueue(List<Vector3> positions, ParticleSystemName name)
    {
        particlePrefabs.Single(p => p.ParticleSystemName == name).ParticleQueue.AddRange(positions);
    }

    /// <summary>
    /// Sends all the particlequeues to the particle generator
    /// </summary>
    private void ExecuteParticleQueue()
    {
        foreach (ParticlePrefabStruct par in particlePrefabs)
        {
            if (par.ParticleQueue.Count() > 0)
            {
                GenerateParticles(par.ParticleQueue, par.ParticleSystemName);
                par.ParticleQueue.Clear();
            }
        }
    }

    /// <summary>
    /// Tells the particle systems where to spawn new particles
    /// </summary>
    /// <param name="particlePos"></param>
    public void GenerateParticles(List<Vector3> particlePos, ParticleSystemName name)
    {
        //Debug.Log($"Particles this frame: {particlePos.Count()}");

        if (particlePos.Count() > particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem * 0.9f && p.ParticleSystemName == name).ToList().Count())
        {
            FillParticlePool(particlePos.Count() - particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem * 0.9f && p.ParticleSystemName == name).ToList().Count(), name);
        }

        List<ParticleStruct> temp = particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem * 0.9f && p.ParticleSystemName == name).ToList();

        for (int i = 0; i < temp.Count() && i < particlePos.Count(); i++)
        {
            AddOneParticle(temp[i], particlePos[i]);
        }
    }

    /// <summary>
    /// Generates one particle in one particle system
    /// </summary>
    /// <param name="particleStruct">the particle system</param>
    /// <param name="pos">the position of the particle</param>
    private void AddOneParticle(ParticleStruct particleStruct, Vector3 pos)
    {
        totalSpawnedParticles++;
        //particleStruct.CurrentParticleAmount++;
        particleStruct.VFX.SetVector3("ParticlePos", pos);
        particleStruct.VFX.SendEvent("CreateParticle");

        if (enableDebug)
        {
            Instantiate(particleDebug, pos, Quaternion.identity);
        }
    }
    #endregion

    #region ParticleUpdates
    private void UpdateMainParticleGradient()
    {
        switch (EnemyManager.Instance.GetCurrentAlertState())
        {
            case EnemyAlertState.Idle:
                currentGradient = GradientExtension.Lerp(currentGradient, idleGradient, lerpTime * Time.deltaTime);
                break;
            case EnemyAlertState.Sus:
                currentGradient = GradientExtension.Lerp(currentGradient, susGradient, lerpTime * Time.deltaTime);
                break;
            case EnemyAlertState.Alerted:
                currentGradient = GradientExtension.Lerp(currentGradient, alertedGradient, lerpTime * Time.deltaTime);
                break;
        }

        foreach (ParticleStruct par in particlePool.Where(p => p.ParticleSystemName == ParticleSystemName.Player))
        {
            par.VFX.SetGradient("ColorGradient", currentGradient);
        }
    }

    /// <summary>
    /// Updates the positions of the particle systems, so they staay inside the cameraview
    /// </summary>
    private void UpdateParticleSystemPos()
    {
        foreach (ParticleStruct par in particlePool)
        {
            par.VFX.transform.position = particleParent.position;
        }
    }

    private void UpdateLivingParticleCounter()
    {
        for (int i = 0; i < 10; i++)
        {
            particlePool[ParticlePoolIndex].CurrentParticleAmount = particlePool[ParticlePoolIndex].VFX.aliveParticleCount;
            ParticlePoolIndex++;
        }
    }

    public void PlayerIsDead(float value)
    {
        foreach (ParticleStruct str in particlePool)
        {
            str.VFX.SetFloat("IsDead", value);
        }
    }
    #endregion

    #region PlayerParticles
    /// <summary>
    /// Generates a list of particles in random directions at the colliders around
    /// </summary>
    /// <returns></returns>
    private List<Vector3> GenerateParticlePositions()
    {
        List<Vector3> temp = new List<Vector3>();

        for (int i = 0; i < GameManager.Instance.PlayerController.ShootRate * Time.deltaTime; i++)
        {
            Vector3 shootDir = GenerateShootDirection();

            //Debug.DrawRay(cam.transform.position, shootDir.normalized, Color.red, 0.25f);

            RaycastHit hit = PhysicsExtension.RaycastFirst(cam.transform.position, shootDir, 100f, particleLayerMask);

            if (hit.collider != null)
            {
                if (!hit.collider.gameObject.IsInLayerMask(particleBlockMask))
                {
                    temp.Add(hit.point);
                }
            }
        }

        return temp;
    }

    /// <summary>
    /// Generates a random shoot direction
    /// </summary>
    /// <returns></returns>
    private Vector3 GenerateShootDirection()
    {
        return ((cam.transform.position + cam.transform.forward * 1000).normalized) + new Vector3(Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100, Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100, Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100);
    }
    #endregion

    #region ParticlePool
    /// <summary>
    /// Generates more particle systems
    /// </summary>
    /// <param name="amount"></param>
    private void FillParticlePool(int amount, ParticleSystemName particleSystemName)
    {
        for (int i = 0; i < amount; i++)
        {
            GenerateParticleStruct(particleSystemName);
        }
    }

    /// <summary>
    /// Generates one particle system with the relevant struct around it
    /// </summary>
    private void GenerateParticleStruct(ParticleSystemName particleSystemName)
    {
        VisualEffect effect = Instantiate(particlePrefabs.Single(p => p.ParticleSystemName == particleSystemName).VFXPrefab, Vector3.zero, Quaternion.identity);

        effect.transform.SetParent(transform);
        effect.transform.localPosition = Vector3.zero;

        ParticleStruct particleStruct = new ParticleStruct(effect, particleSystemName);

        particlePool.Add(particleStruct);
    }

    /// <summary>
    /// Sets the transform the particle systems should follow
    /// </summary>
    /// <param name="parent"></param>
    public void SetParticleParent(Transform parent)
    {
        if (particleParent == null)
        {
            particleParent = parent;
        }
    }
    #endregion
}

[System.Serializable]
public class ParticleStruct
{
    public VisualEffect VFX;
    public int CurrentParticleAmount;
    public ParticleSystemName ParticleSystemName;

    public ParticleStruct(VisualEffect vfx, ParticleSystemName particleSystemName)
    {
        VFX = vfx;
        CurrentParticleAmount = 0;
        ParticleSystemName = particleSystemName;
    }
}

[System.Serializable]
public struct ParticlePrefabStruct
{
    public VisualEffect VFXPrefab;
    public ParticleSystemName ParticleSystemName;
    [HideInInspector] public List<Vector3> ParticleQueue;
}

public enum ParticleSystemName
{
    Player,
    AutoScanners
}