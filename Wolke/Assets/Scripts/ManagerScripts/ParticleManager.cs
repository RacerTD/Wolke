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
    [SerializeField] private VisualEffect particleSystemPrefab;
    [SerializeField] private int maxParticlesInSystem;
    [SerializeField] private int totalSpawnedParticles = 0;
    [SerializeField] private LayerMask particleLayerMask;

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

    private Transform particleParent;
    private bool particleGeneratorActive = false;
    public void SetParticleGeneratorState(bool value) => particleGeneratorActive = value;
    private Camera cam;

    public void Start()
    {
        cam = Camera.main;
    }

    public void Update()
    {
        UpdateParticleSystemPos();

        if (particleGeneratorActive)
        {
            GenerateParticles(GenerateParticlePositions());
        }
    }

    private void UpdateParticleSystemPos()
    {
        foreach (ParticleStruct par in particlePool)
        {
            par.VFX.transform.position = particleParent.position;
        }
    }

    private List<Vector3> GenerateParticlePositions()
    {
        List<Vector3> temp = new List<Vector3>();

        for (int i = 0; i < GameManager.Instance.PlayerController.ShootRate * Time.deltaTime; i++)
        {
            Vector3 shootDir = GenerateShootDirection();

            Debug.DrawRay(cam.transform.position, shootDir.normalized, Color.red, 0.25f);

            RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, shootDir, 100f, particleLayerMask);

            hits = hits.OrderBy(h => (h.point - cam.transform.position).magnitude).ToArray();

            if (hits.Length > 0)
            {
                temp.Add(hits[0].point);
            }
        }

        return temp;
    }

    private Vector3 GenerateShootDirection()
    {
        return ((cam.transform.position + cam.transform.forward * 1000).normalized) + new Vector3(Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100, Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100, Random.Range(GameManager.Instance.PlayerController.ParticleSpread, -GameManager.Instance.PlayerController.ParticleSpread) / 100);
    }

    private void GenerateParticles(List<Vector3> particlePos)
    {
        //Debug.Log($"Particles this frame: {particlePos.Count()}");

        if (particlePos.Count() > particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList().Count())
        {
            FillParticlePool(particlePos.Count() - particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList().Count());
        }

        List<ParticleStruct> temp = particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList();

        for (int i = 0; i < temp.Count() && i < particlePos.Count(); i++)
        {
            AddOneParticle(temp[i], particlePos[i]);
        }
    }

    private void AddOneParticle(ParticleStruct particleStruct, Vector3 pos)
    {
        totalSpawnedParticles++;
        particleStruct.CurrentParticleAmount++;
        particleStruct.VFX.SetVector3("ParticlePos", pos);
        particleStruct.VFX.SendEvent("CreateParticle");

        if (enableDebug)
        {
            Instantiate(particleDebug, pos, Quaternion.identity);
        }
    }

    private void FillParticlePool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GenerateParticleStruct();
        }
    }

    private void GenerateParticleStruct()
    {
        VisualEffect effect = Instantiate(particleSystemPrefab, Vector3.zero, Quaternion.identity);
        effect.transform.SetParent(transform);
        effect.transform.localPosition = Vector3.zero;

        ParticleStruct particleStruct = new ParticleStruct(effect);

        particlePool.Add(particleStruct);
    }

    public void SetParticleParent(Transform parent)
    {
        if (particleParent == null)
        {
            particleParent = parent;
        }
    }
}

[System.Serializable]
public struct ParticleStruct
{
    public VisualEffect VFX;
    public int CurrentParticleAmount;

    public ParticleStruct(VisualEffect vfx)
    {
        VFX = vfx;
        CurrentParticleAmount = 0;
    }
}
