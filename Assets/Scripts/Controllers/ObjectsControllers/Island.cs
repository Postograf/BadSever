using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Island : ObjectsController
{
    [SerializeField] private List<Health> _houses;

    private void Start()
    {
        foreach (var house in _houses)
        {
            AddObject(house);
        }
    }

    public Health ClosestTarget(Vector3 point)
    {
        return _houses
            .Where(h => h != null && h.Current > 0)
            .OrderBy(h => Ruler.SqrDistance(h.Hitbox, point))
            .FirstOrDefault();
    }

    public override void Refresh()
    {
        foreach (var house in _spawnedObjects)
        {
            house.FullRecover();
        }
    }
}