using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AllySpawner : ObjectsController
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Unit _prefab;

    private void Start()
    {
        var rotation  = Quaternion.identity;

        foreach (var spawnPoint in _spawnPoints)
        {
            var unit = Instantiate(_prefab, spawnPoint.position, rotation);

            if (unit.TryGetComponent(out Health health))
            {
                AddObject(health);
            }
        }
    }

    public override void Refresh()
    {
        base.Refresh();

        foreach (var unit in _spawnedObjects.Where(x => x != null))
        {
            Destroy(unit.gameObject);
        }

        _spawnedObjects.Clear();
        _aliveUnits = 0;

        Start();
    }
}
