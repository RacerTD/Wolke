using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCamera : AbilityController
{
    [SerializeField] private Vector3 axisMovementMult = Vector3.zero;
    [SerializeField] private Vector3 axisPositions = Vector3.zero;
    [SerializeField] private Transform particleShootPoint;
    [SerializeField] private LayerMask particleColliders;
    [SerializeField] private Vector3 particleSpread = new Vector3(10, 20, 30);
    [SerializeField] private float particlesPerSecond = 200f;

    protected override void Update()
    {
        UpdateAxisPositions();

        ParticleManager.Instance.GenerateParticles(GenerateParticlePos(), ParticleSystemName.AutoScanners);

        base.Update();
    }

    private void UpdateAxisPositions()
    {
        axisPositions = new Vector3(axisPositions.x + axisMovementMult.x * Time.deltaTime, axisPositions.y + axisMovementMult.y * Time.deltaTime, axisPositions.z + axisMovementMult.z * Time.deltaTime);

        particleShootPoint.localRotation = Quaternion.Euler(axisPositions);

        Debug.DrawRay(particleShootPoint.position, particleShootPoint.forward, Color.blue);
        Debug.DrawRay(particleShootPoint.position, particleShootPoint.right * 0.5f, Color.blue);
    }

    private List<Vector3> GenerateParticlePos()
    {
        List<Vector3> temp = new List<Vector3>();
        for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
        {
            Vector3 shootDir = GenerateShootDirection();

            Debug.DrawRay(particleShootPoint.position, shootDir.normalized, Color.red, 0.25f);

            RaycastHit[] hits = Physics.RaycastAll(particleShootPoint.position, shootDir, 100f, particleColliders);

            hits = hits.OrderBy(h => (h.point - particleShootPoint.position).magnitude).ToArray();

            if (hits.Length > 0)
            {
                temp.Add(hits[0].point);
            }
        }

        return temp;
    }

    private Vector3 GenerateShootDirection()
    {
        return new Vector3(particleShootPoint.forward.x + Random.Range(-particleSpread.x, particleSpread.x), particleShootPoint.forward.y + Random.Range(-particleSpread.y, particleSpread.y), particleShootPoint.forward.z + Random.Range(-particleSpread.z, particleSpread.z));
    }
}
