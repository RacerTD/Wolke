using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour
{
    private ObjectPool pool;
    [SerializeField] private GameObject PoolObject;

    public void Start()
    {
        pool = ObjectPool.CreateNewObjectPool(PoolObject);
    }
}
