using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ruler
{
    public static float SqrDistance(Collider collider, Vector3 position)
    {
        var closestPoint = collider.ClosestPoint(position);
        return (closestPoint - position).sqrMagnitude;
    }
}