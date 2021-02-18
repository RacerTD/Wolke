using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    [SerializeField] private AutoScannerMode autoScannerMode = AutoScannerMode.RoundAndRound;
    [SerializeField] private float particlesPerSecond = 100f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float verticlalSpread = 40f;
    [SerializeField] [Tooltip("Everything the particles can hit")] private LayerMask particleLayerMask;
    [SerializeField] [Tooltip("Everything particles can not stick to")] private LayerMask particleBlockMask;

    [SerializeField] private float startDegree = -20f;
    [SerializeField] private float endDegree = 20f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool currentlyActive = false;
    public bool CurrentlyActive
    {
        get => currentlyActive;
        set
        {
            if(value != currentlyActive)
            {
                UpdateActiveState();
                currentlyActive = value;

                if(value)
                {
                    startBaseRotation = shootPoint.rotation.eulerAngles;
                }
            }
        }
    }
    private void UpdateActiveState()
    {
        switch(autoScannerMode)
        {
            case AutoScannerMode.FromToDegree:
                currentBaseRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);
                shootPoint.localRotation = Quaternion.Euler(currentBaseRotation);
                break;
            case AutoScannerMode.FromToAndBack:
                currentBaseRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);
                currentRotation = shootPoint.localRotation.eulerAngles;
                break;
            case AutoScannerMode.RoundAndRound:            
                break;
        }
    }

    [Header("Other")]
    [SerializeField] private Transform shootPoint;
    private Vector3 currentBaseRotation = Vector3.zero;
    private Vector3 startBaseRotation = Vector3.zero;
    private Vector3 currentRotation = Vector3.zero;
    private Vector3 originalStartRotation = Vector3.zero;

    private void Start()
    {
        originalStartRotation = shootPoint.localRotation.eulerAngles;
        currentBaseRotation = shootPoint.localRotation.eulerAngles;

        UpdateActiveState();
    }

    private void Update()
    {
        switch(autoScannerMode)
        {
            case AutoScannerMode.FromToDegree:
                UpdateFromToDegree();
                break;
            case AutoScannerMode.FromToAndBack:
                UpdateFromToAndBack();
                break;
            case AutoScannerMode.RoundAndRound:
                UpdateRoundAndRound();
                break;
        }
    }

    private void UpdateFromToDegree()
    {
        if (CurrentlyActive)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
            {
                Vector3 shootDir = GenerateShootDirection();

                RaycastHit hit = PhysicsExtension.RaycastFirst(shootPoint.transform.position, shootDir, 100f, particleLayerMask);

                if (hit.collider != null)
                {
                    if (!hit.collider.gameObject.IsInLayerMask(particleBlockMask))
                    {
                        temp.Add(hit.point);
                    }
                }
            }
            ParticleManager.Instance.AddParticlesToQueue(temp, ParticleSystemName.AutoScanners);

            currentRotation = new Vector3(currentRotation.x, currentRotation.y + rotationSpeed * Time.deltaTime, currentRotation.z);
            currentBaseRotation = currentRotation;

            if(currentRotation.y >= endDegree)
            {
                currentRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);
            }

            shootPoint.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private void UpdateFromToAndBack()
    {
        if (CurrentlyActive)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
            {
                Vector3 shootDir = GenerateShootDirection();

                RaycastHit hit = PhysicsExtension.RaycastFirst(shootPoint.transform.position, shootDir, 100f, particleLayerMask);

                if (hit.collider != null)
                {
                    if (!hit.collider.gameObject.IsInLayerMask(particleBlockMask))
                    {
                        temp.Add(hit.point);
                    }
                }
            }
            ParticleManager.Instance.AddParticlesToQueue(temp, ParticleSystemName.AutoScanners);

            currentRotation = new Vector3(currentRotation.x, currentRotation.y + rotationSpeed * Time.deltaTime, currentRotation.z);
            currentBaseRotation = currentRotation;

            if (currentRotation.y > endDegree || currentRotation.y < startDegree)
            {
                rotationSpeed = -rotationSpeed;
            }

            shootPoint.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private void UpdateRoundAndRound()
    {
        if(CurrentlyActive)
        {
            List<Vector3> temp = new List<Vector3>();

            for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
            {
                Vector3 shootDir = GenerateShootDirection();

                //Debug.DrawRay(cam.transform.position, shootDir.normalized, Color.red, 0.25f);

                RaycastHit hit = PhysicsExtension.RaycastFirst(shootPoint.transform.position, shootDir, 100f, particleLayerMask);

                if (hit.collider != null)
                {
                    if (!hit.collider.gameObject.IsInLayerMask(particleBlockMask))
                    {
                        temp.Add(hit.point);
                    }
                }
            }

            ParticleManager.Instance.AddParticlesToQueue(temp, ParticleSystemName.AutoScanners);

            currentRotation = new Vector3(currentRotation.x, currentRotation.y + rotationSpeed * Time.deltaTime, currentRotation.z);
            currentBaseRotation = currentRotation;

            shootPoint.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private Vector3 GenerateShootDirection()
    {
        shootPoint.localRotation = Quaternion.Euler(currentBaseRotation.x + Random.Range(-verticlalSpread, verticlalSpread), currentBaseRotation.y, currentBaseRotation.z);
        return shootPoint.forward;
    }
}

public enum AutoScannerMode
{
    FromToDegree,
    FromToAndBack,
    RoundAndRound
}
