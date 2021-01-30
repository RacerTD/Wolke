using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleParent : MonoBehaviour
{
    void Start()
    {
        ParticleManager.Instance.SetParticleParent(transform);
    }
}
