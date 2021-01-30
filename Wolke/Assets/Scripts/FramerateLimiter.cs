using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLimiter : MonoBehaviour
{
    public int TargetFPS = 30;
    
    void Start()
    {
        Application.targetFrameRate = TargetFPS;
    }
}
