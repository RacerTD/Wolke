using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    public Vector3 Axis = Vector3.zero;
    public Vector3 StartOffset = Vector3.zero;

    private void Awake()
    {
        StartOffset = transform.localPosition;
    }
}
