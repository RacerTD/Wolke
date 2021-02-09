using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionCollider : MonoBehaviour
{
    private EnemyController controller;

    private void Start()
    {
        if (GetComponentInParent<EnemyController>() != null)
            controller = GetComponentInParent<EnemyController>();
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            controller.SetDataDetectionCollider(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            controller.SetDataDetectionCollider(false);
    }
}
