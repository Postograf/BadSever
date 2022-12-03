using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CameraController))]
public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private float _maxRaycastDistance;

    private Camera _camera;
    private CameraController _cameraController;

    private Unit _unit;
    private Vector3 _oldMousePosition;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _maxRaycastDistance, _raycastMask))
            {
                if (_unit != null)
                {
                    _unit.Relocate(hit.point);
                    _unit.Select(false);
                    _unit = null;
                }
                else if (hit.collider.TryGetComponent(out Unit unit) && unit.IsDead == false)
                {
                    _unit = unit;
                    _unit.Select(true);
                }
            }
        }

        if  (Input.GetMouseButtonDown(1))
        {
            _oldMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            _cameraController.Rotate(Input.mousePosition.x - _oldMousePosition.x);
            _oldMousePosition = Input.mousePosition;
        }

        var scroll = Input.mouseScrollDelta;
        if (scroll.y != 0)
        {
            _cameraController.Zoom(-scroll.y);
        }
    }
}