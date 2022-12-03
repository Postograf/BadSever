using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationFriction;

    [Header("Distance")]
    [SerializeField] private Vector2 _minMaxDistance;
    [SerializeField] private float _distance;
    [SerializeField] private float _distancePerZoom;

    private Transform _transform;
    private Vector3 _rotationAxis;
    private float _rotationVelocity;

    public float Distance
    {
        get => _distance;
        set => _distance = Mathf.Clamp(value, _minMaxDistance.x, _minMaxDistance.y);
    }

    private void Start()
    {
        _transform = transform;
        _rotationAxis = Vector3.up;
    }

    private void Update()
    {
        if (_target != null)
        {
            _transform.LookAt(_target.position);
            _transform.position = _target.position - _transform.forward * Distance;

            if (_rotationVelocity != 0)
            {
                _transform.RotateAround(_target.position, _rotationAxis, _rotationVelocity * Time.deltaTime);
                var sign = _rotationVelocity.Sign();
                _rotationVelocity -= sign * _rotationFriction * Time.deltaTime;
                var sign2 = _rotationVelocity.Sign();

                if (sign != sign2)
                    _rotationVelocity = 0;
            }
        }
    }

    public void Rotate(float rotationSpeedMultipier)
    {
        _rotationVelocity = _rotationSpeed * rotationSpeedMultipier;
    }

    public void Zoom(float direction)
    {
        Distance += direction * _distancePerZoom;
    }
}
