using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

public class WaveSpawner : ObjectsController
{
    [SerializeField] private Boat _boatPrefab;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private int _minUnitsOnBoat = 1;
    [SerializeField] private float _rangeFromIsland;

    private List<List<Boat>> _boatsWaves;
    private Island _island;
    private Coroutine _delayedSpawner;

    private void Start()
    {
        _boatsWaves = new List<List<Boat>>(_waves.Count);
        _island = FindObjectOfType<Island>();

        foreach (var wave in _waves)
        {
            var requiredBoatsCount = Mathf.CeilToInt(1f * wave.UnitsCount / _boatPrefab.Capacity);
            var boatsCount = Mathf.Max(wave.PreferredBoatsCount, requiredBoatsCount);
            if (wave.UnitsCount / boatsCount < _minUnitsOnBoat)
                boatsCount = requiredBoatsCount;

            var boats = InstantiateBoats(boatsCount);

            //Сначала заполняем все корабли по 1 юниту потом до максимума
            var boatIndex = 0;
            for (int i = 0; i < wave.UnitsCount; i++)
            {
                var boat = boats[boatIndex];
                var unit = Instantiate(_unitPrefab);

                if (unit.TryGetComponent(out HouseFinder houseFinder))
                {
                    houseFinder.Island = _island;
                }

                if (unit.TryGetComponent(out Health health))
                {
                    AddObject(health);
                }

                boat.TryAddUnit(unit);
                boat.gameObject.SetActive(false);

                if (boat.UnitsCount == 1 || boat.UnitsCount == boat.Capacity)
                {
                    boatIndex++;
                    boatIndex %= boats.Count;
                }
            }
            _boatsWaves.Add(boats);
        }

        _delayedSpawner = StartCoroutine(DelayedSpawner());
    }

    private IEnumerator DelayedSpawner()
    {
        var islandCenter = _island?.transform.position ?? Vector3.zero;

        for (int i = 0; i < _boatsWaves.Count; i++)
        {
            var wave = _boatsWaves[i];
            yield return new WaitForSeconds(_waves[i].DelayBefore);

            var randomX = Random.Range(-1f, 1f);
            var randomZ = Random.Range(-1f, 1f);
            var position = islandCenter + new Vector3(randomX, 0, randomZ).normalized * _rangeFromIsland;
            var maxRotation = 360 / wave.Count;
            var minRotation = _boatPrefab.GetComponent<Collider>()?.bounds.size.x ?? 0 / 2;
            var axis = Vector3.up;
            foreach (var boat in wave)
            {
                var boatTransform = boat.transform;
                boatTransform.position = position;
                boatTransform.LookAt(islandCenter);

                boat.gameObject.SetActive(true);

                if (boat.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.velocity = boatTransform.forward * boat.Speed;

                var angle = Random.Range(minRotation, maxRotation);
                position = Quaternion.AngleAxis(angle, axis) * position;
            }
        }
    }

    private List<Boat> InstantiateBoats(int count)
    {
        count = Mathf.Max(0, count);
        var result = new List<Boat>(count);

        for (int i = 0; i < count; i++)
        {
            var boat = Instantiate(_boatPrefab);
            boat.Init();
            result.Add(boat);
        }

        return result;
    }

    public override void Refresh()
    {
        base.Refresh();

        foreach (var unit in _spawnedObjects.Where(x => x != null))
            Destroy(unit.gameObject);
        _spawnedObjects.Clear();
        _aliveUnits = 0;

        foreach (var wave in _boatsWaves)
            foreach(var boat in wave)
                Destroy(boat.gameObject);
        _boatsWaves.Clear();

        StopCoroutine(_delayedSpawner);
        _delayedSpawner = null;

        Start();
    }
}
