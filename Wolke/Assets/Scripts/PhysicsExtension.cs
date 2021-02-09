using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PhysicsExtension
{
    /// <summary>
    /// Return the first object it hits
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static RaycastHit RaycastFirst(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
    {
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance, layerMask);

        hits.OrderBy(h => Vector3.Distance(h.point, origin)).ToArray();

        return hits.FirstOrDefault();
    }
}
