using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public abstract class ObjectsController : MonoBehaviour
{
    protected List<Health> _spawnedObjects = new List<Health>();
    protected int _aliveUnits;

    public IEnumerable<Health> SpawnedObjects => _spawnedObjects.Select(x => x);
    public event Action AllObjectsDead;
    public event Action<Health> ObjectSpawned;
    public event Action<Health> ObjectDespawned;

    protected virtual void AddObject(Health health)
    {
        _spawnedObjects.Add(health);
        _aliveUnits = _spawnedObjects.Count;
        ObjectSpawned?.Invoke(health);
        health.Dead += OnObjectDead;
    }

    protected virtual void OnObjectDead(Health health)
    {
        _aliveUnits--;
        ObjectDespawned?.Invoke(health);

        if (_aliveUnits <= 0)
            AllObjectsDead?.Invoke();
    }

    public virtual void Refresh()
    {
        foreach (var health in _spawnedObjects)
        {
            ObjectDespawned?.Invoke(health);
        }
    }
}
