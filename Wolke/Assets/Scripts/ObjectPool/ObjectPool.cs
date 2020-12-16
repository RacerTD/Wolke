using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool : MonoBehaviour
{
    public static List<ObjectPool> Pools;
    private List<GameObject> pooledObjects;
    public ObjectPoolMode PoolMode = ObjectPoolMode.Growing;
    public string PoolItemName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform fixedParent;
    [SerializeField] private bool useFixedLayer;
    [SerializeField, Layer] private int fixedLayer;

    [SerializeField, Limit(1)] private int size = 1;
    public int Size
    {
        get => size;
        set => size = Math.Max(1, value);
    }

    [SerializeField, Limit(1)] private int growAmount = 1;
    public int GrowAmount
    {
        get => growAmount;
        set => growAmount = Math.Max(1, value);
    }

    public ObjectPoolEvent OnSpawn;
    public ObjectPoolEvent OnRecycle;

    public bool IsFull => pooledObjects.All(g => g.activeInHierarchy);
    public int ActivePoolObjects => pooledObjects.Where(g => g.activeInHierarchy).Count();

    static ObjectPool()
    {
        Pools = new List<ObjectPool>();
    }

    protected virtual void Start()
    {
        Pools.Add(this);
    }

    /// <summary>
    /// The setup for the objectPool
    /// </summary>
    public void SetUp(GameObject obj)
    {
        prefab = obj;
        PoolItemName = obj.name;
        pooledObjects = new List<GameObject>(Size);
        for (int i = 0; i < Size; i++)
        {
            SpawnNewPoolItem();
        }
    }

    /// <summary>
    /// Spawns an entire new poolObject
    /// </summary>
    private void SpawnNewPoolItem()
    {
        GameObject obj = Instantiate(prefab, transform);

        if (fixedParent != null)
            obj.transform.SetParent(fixedParent);
        if (useFixedLayer)
            obj.layer = fixedLayer;

        obj.name = PoolItemName;
        obj.SetActive(false);
        pooledObjects.Add(obj);
    }

    /// <summary>
    /// Returns one of the pool objects and sets the pos etc.
    /// </summary>
    public GameObject Spawn(Vector3 position, Quaternion? rotation = null, Vector3? scale = null)
    {
        if (IsFull)
        {
            switch (PoolMode)
            {
                case ObjectPoolMode.FixedSize:
                    throw new InvalidOperationException($"Trying to spawn a new object from fixed-sized pool {name} failed");
                case ObjectPoolMode.Growing:
                    Grow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GameObject obj = pooledObjects.First(g => !g.activeInHierarchy);
        obj.transform.position = position;
        obj.transform.rotation = rotation ?? Quaternion.identity;
        obj.transform.localScale = scale ?? Vector3.one;
        obj.SetActive(true);
        OnSpawn.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Adds more poolObjects to the objectPool
    /// </summary>
    private void Grow()
    {
        for (int i = 0; i < GrowAmount; i++)
        {
            SpawnNewPoolItem();
        }
    }

    /// <summary>
    /// Disables the poolObject
    /// </summary>
    public void Recycle(GameObject obj)
    {
        if (obj != null && pooledObjects.Any(g => g == obj))
        {
            obj.SetActive(false);
            OnRecycle.Invoke(obj);
        }
    }

    private void OnDestroy()
    {
        Pools.Remove(this);
    }

    public static ObjectPool CreateNewObjectPool(GameObject obj)
    {
        GameObject temp = new GameObject("New unnamed ObjectPool");
        temp.AddComponent<ObjectPool>();
        temp.GetComponent<ObjectPool>().SetUp(obj);
        return temp.GetComponent<ObjectPool>();
    }

    public static ObjectPool CreateNewObjectPool(GameObject obj, string name)
    {
        GameObject temp = new GameObject(name);
        temp.AddComponent<ObjectPool>();
        temp.GetComponent<ObjectPool>().SetUp(obj);
        return temp.GetComponent<ObjectPool>();
    }
}

public enum ObjectPoolMode
{
    FixedSize,
    Growing
}

[Serializable]
public class ObjectPoolEvent : UnityEvent<GameObject>
{

}
