using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.SetRespawnPoint(this);
    }
}
