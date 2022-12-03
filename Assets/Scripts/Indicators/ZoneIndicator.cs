using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ZoneIndicator : MonoBehaviour
{
    [SerializeField] private int _segmentsCount = 128;
    [SerializeField] private float _lineUpOffset = 0.2f;

    private LineRenderer _lineRenderer;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public void CalculteLine(float radius, bool isStatic, Vector3? staticPosition = null)
    {
        _lineRenderer.positionCount = _segmentsCount + 1;
        _lineRenderer.useWorldSpace = isStatic;

        var deltaTheta = 2 * Mathf.PI / _segmentsCount;
        var theta = 0f;

        var reversedRotation = _transform.rotation;
        reversedRotation.w *= -1;

        for (int i = 0; i < _segmentsCount + 1; i++)
        {
            var x = radius * Mathf.Cos(theta);
            var z = radius * Mathf.Sin(theta);
            var position = new Vector3(x, _lineUpOffset, z);

            if (isStatic == false)
                position = reversedRotation * position;
            else if (staticPosition.HasValue)
                position += staticPosition.Value;

            _lineRenderer.SetPosition(i, position);
            theta += deltaTheta;
        }
    }

    public void Show(bool state)
    {
        _lineRenderer.enabled = state;
    }
}
