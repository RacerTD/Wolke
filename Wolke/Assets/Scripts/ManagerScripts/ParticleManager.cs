using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : ManagerModule<ParticleManager>
{
    [SerializeField] private List<ParticleStruct> particlePool = new List<ParticleStruct>();
    private int currentParticlePoolIndex = 0;
    public int CurrentParticlePoolIndex
    {
        get => currentParticlePoolIndex;
        set
        {
            if (value > particlePool.Count())
            {
                currentParticlePoolIndex = 0;
            }
            else
            {
                currentParticlePoolIndex = value;
            }
        }
    }
    [SerializeField] private VisualEffect particleSystemPrefab;
    [SerializeField] private int maxParticlesInSystem;
    private Transform particleParent;

    public void Start()
    {

    }

    public void GenerateParticles(List<Vector3> particlePos)
    {
        if (particlePos.Count() > particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList().Count())
        {
            FillParticlePool(particlePos.Count() - particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList().Count());
        }

        List<ParticleStruct> temp = particlePool.Where(p => p.CurrentParticleAmount < maxParticlesInSystem).ToList();

        //Debug.Log($"TempCount: {temp.Count()}");

        for (int i = 0; i < temp.Count() && i < particlePos.Count(); i++)
        {
            //Debug.Log($"i: {i} Count: {temp.Count()} particlePosCount: {particlePos.Count()} ParticlePoolCount: {particlePool.Count()}");

            AddOneParticle(temp[i], particlePos[i]);
        }
    }

    private void AddOneParticle(ParticleStruct particleStruct, Vector3 pos)
    {
        //Debug.Log(pos);
        particleStruct.CurrentParticleAmount++;
        particleStruct.VFX.SetVector3("ParticlePos", pos);
        particleStruct.VFX.SendEvent("CreateParticle");
    }

    public void SetParticleParent(Transform parent)
    {
        if (particleParent == null)
        {
            particleParent = parent;
        }
    }

    private void GenerateParticleStruct()
    {
        VisualEffect effect = Instantiate(particleSystemPrefab, Vector3.zero, Quaternion.identity);
        effect.transform.SetParent(particleParent);

        ParticleStruct particleStruct = new ParticleStruct(effect);

        particlePool.Add(particleStruct);
    }

    private void FillParticlePool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GenerateParticleStruct();
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
