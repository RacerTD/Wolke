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
            if (value != currentlyActive)
            {
                UpdateActiveState();
                currentlyActive = value;

                if (value)
                {
                    startBaseRotation = shootPoint.rotation.eulerAngles;
                }
            }
        }
    }
    private void UpdateActiveState()
    {
        switch (autoScannerMode)
        {
            case AutoScannerMode.FromToDegree:
                CurrentBaseRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);
                shootPoint.localRotation = Quaternion.Euler(CurrentBaseRotation);
                break;
            case AutoScannerMode.FromToAndBack:
                CurrentBaseRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);
                currentRotation = shootPoint.localRotation.eulerAngles;
                break;
            case AutoScannerMode.RoundAndRound:
                break;
        }
    }

    [Header("Other")]
    [SerializeField] private Transform shootPoint;
    [HideInInspector] public Vector3 CurrentBaseRotation = Vector3.zero;
    private Vector3 startBaseRotation = Vector3.zero;
    private Vector3 currentRotation = Vector3.zero;
    private Vector3 originalStartRotation = Vector3.zero;

    private void Start()
    {
        if (!EnemyManager.Instance.AutoScanner.Contains(this))
        {
            EnemyManager.Instance.AutoScanner.Add(this);
        }

        originalStartRotation = shootPoint.localRotation.eulerAngles;
        CurrentBaseRotation = shootPoint.localRotation.eulerAngles;

        UpdateActiveState();
    }

    private void Update()
    {
        if (Time.time >= 10f)
            switch (autoScannerMode)
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
                case AutoScannerMode.Cone:
                    UpdateCone();
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
            CurrentBaseRotation = currentRotation;

            if (currentRotation.y >= endDegree)
            {
                currentRotation = new Vector3(originalStartRotation.x, originalStartRotation.y + startDegree, originalStartRotation.z);

                if (!loop)
                {
                    CurrentlyActive = false;
                }
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
            CurrentBaseRotation = currentRotation;

            if (currentRotation.y > endDegree || currentRotation.y < startDegree)
            {
                rotationSpeed = -rotationSpeed;

                if (!loop && rotationSpeed > 0)
                {
                    CurrentlyActive = false;
                }
            }

            shootPoint.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private void UpdateRoundAndRound()
    {
        if (CurrentlyActive)
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
            CurrentBaseRotation = currentRotation;

            shootPoint.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private void UpdateCone()
    {
        if (CurrentlyActive)
        {
            List<Vector3> temp = new List<Vector3>();

            for (int i = 0; i < particlesPerSecond * Time.deltaTime; i++)
            {
                Vector3 shootDir = GenerateShootDirectionConeMode();

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
        }
    }

    private Vector3 GenerateShootDirection()
    {
        shootPoint.localRotation = Quaternion.Euler(CurrentBaseRotation.x + Random.Range(-verticlalSpread, verticlalSpread), CurrentBaseRotation.y, CurrentBaseRotation.z);
        return shootPoint.forward;
    }

    private Vector3 GenerateShootDirectionConeMode()
    {
        shootPoint.localRotation = Quaternion.Euler(CurrentBaseRotation.x + Random.Range(-verticlalSpread, verticlalSpread), CurrentBaseRotation.y + Random.Range(-verticlalSpread, verticlalSpread), CurrentBaseRotation.z);
        return shootPoint.forward;
    }
}

public enum AutoScannerMode
{
    FromToDegree,
    FromToAndBack,
    RoundAndRound,
    Cone
}
