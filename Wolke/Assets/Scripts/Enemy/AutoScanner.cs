using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScanner : MonoBehaviour
{
    public Transform ShootPoint;
    [SerializeField] private float particlesPerSecond = 100f;
    public float DegreeChangeHorizontal = 10f;
    private Vector3 currentBaseRotation = Vector3.zero;
    public float VerticalSpread = 40f;
    [SerializeField] [Tooltip("Everything the particles can hit")] private LayerMask particleLayerMask;
    [SerializeField] [Tooltip("Everything particles can not stick to")] private LayerMask particleBlockMask;

    private void Start()
    {
        currentBaseRotation = ShootPoint.rotation.eulerAngles;
    }

    private void Update()
    {
        ShootPoint.rotation = Quaternion.Euler(ShootPoint.rotation.eulerAngles.x, ShootPoint.rotation.eulerAngles.y + DegreeChangeHorizontal * Time.deltaTime, ShootPoint.rotation.eulerAngles.z);

        currentBaseRotation = ShootPoint.rotation.eulerAngles;

        List<Vector3> temp = new List<Vector3>();

        for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
        {
            Vector3 shootDir = GenerateShootDirection();

            //Debug.DrawRay(cam.transform.position, shootDir.normalized, Color.red, 0.25f);

            RaycastHit hit = PhysicsExtension.RaycastFirst(ShootPoint.transform.position, shootDir, 100f, particleLayerMask);

            if (hit.collider != null)
            {
                if (!hit.collider.gameObject.IsInLayerMask(particleBlockMask))
                {
                    temp.Add(hit.point);
                }
            }
        }

        ParticleManager.Instance.AddParticlesToQueue(temp, ParticleSystemName.AutoScanners);

        ShootPoint.rotation = Quaternion.Euler(currentBaseRotation);
    }

    private Vector3 GenerateShootDirection()
    {
        ShootPoint.rotation = Quaternion.Euler(currentBaseRotation.x + Random.Range(-VerticalSpread, VerticalSpread), currentBaseRotation.y, currentBaseRotation.z);
        return ShootPoint.forward;
    }
}
