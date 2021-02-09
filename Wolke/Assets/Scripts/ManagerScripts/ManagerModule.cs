using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerModule<T> : MonoBehaviour where T : ManagerModule<T>
{
    protected static T instance;

    public static T Instance
    {
        get => instance;
    }

    public virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
        else
            Destroy(this);
    }
}
