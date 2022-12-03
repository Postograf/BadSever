using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class HealthBarsController : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBarPrefab;
    [SerializeField] private List<ObjectsController> _healthSpawners;
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private Vector3 _healthBarOffset;

    private List<HealthBar> _healthBars = new List<HealthBar>();
    private Transform _transform;

    private void Start()
    {
        _transform = transform;

        foreach (var spawner in _healthSpawners)
        {
            foreach (var health in spawner.SpawnedObjects)
                SpawnHealthBar(health);
            spawner.ObjectSpawned += SpawnHealthBar;
            spawner.ObjectDespawned += HideHelthBar;
        }
    }

    private HealthBar GetOrSpawnHealthBar()
    {
        var healthBar = _healthBars.FirstOrDefault(h => h.gameObject.activeSelf == false);

        if (healthBar == null)
        {
            healthBar = Instantiate(_healthBarPrefab, _transform);
            _healthBars.Add(healthBar);
        }

        return healthBar;
    }

    private void SpawnHealthBar(Health health)
    {
        GetOrSpawnHealthBar().Init(health);
    }

    private void HideHelthBar(Health health)
    {
        _healthBars.FirstOrDefault(h => h.Health == health)?.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        foreach (var healthBar in _healthBars.Where(h => h.gameObject.activeSelf))
        {
            healthBar.Reposition(_canvas, _healthBarOffset, _camera);
        }
    }
}
