using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthPointUIPrefab;

    private CanvasGroup _group;
    private RectTransform _rectTransform;
    private Transform _transform;
    private Health _health;
    private List<Image> _healthPoints = new List<Image>();

    public Health Health => _health;

    public void Init(Health health)
    {
        _rectTransform ??= GetComponent<RectTransform>();
        _transform ??= transform;
        _group ??= GetComponent<CanvasGroup>();

        _health = health;
        var different = health.Max - _healthPoints.Count;

        if (different < 0)
        {
            different *= -1;
            for (int i = 0; i < _healthPoints.Count; i++)
            {
                _healthPoints[i].gameObject.SetActive(i >= different);
            }
        }
        else
        {
            InstantiatePoints(different);

            foreach (var point in _healthPoints)
                point.gameObject.SetActive(true);
        }

        _health.HealthChanged += OnHealthChanged;
        _health.Dead += OnHealthDead;

        gameObject.SetActive(true);
    }

    private void InstantiatePoints(int count)
    {
        count = Mathf.Max(count, 0);

        for (int i = 0; i < count; i++)
        {
            _healthPoints.Add(Instantiate(_healthPointUIPrefab, _transform));
        }
    }

    private void OnHealthChanged(int current, int max)
    {
        for (int i = 0; i < _healthPoints.Count; i++)
        {
            var color = _healthPoints[i].color;
            color.a = i < current ? 1 : 0;
            _healthPoints[i].color = color;
        }
    }

    private void OnHealthDead(Health health)
    {
        _health.HealthChanged -= OnHealthChanged;
        _health.Dead -= OnHealthDead;
        gameObject.SetActive(false);
    }

    public void Reposition(RectTransform canvas, Vector3 offset, Camera camera)
    {
        _group.alpha = _health.gameObject.activeInHierarchy ? 1 : 0;

        var viewportPosition = camera.WorldToViewportPoint(_health.transform.position + offset);
        var screenPosition = new Vector2(
        ((viewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
        ((viewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));

        _rectTransform.anchoredPosition = screenPosition;
    }
}
