using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class HouseFinder : MonoBehaviour
{
    private Unit _unit;
    private Transform _unitTransform;

    private Health _closestTarget;

    public Island Island { get; set; }

    private void Start()
    {
        _unit = GetComponent<Unit>();
        _unitTransform = _unit?.transform ?? transform;
    }

    private void Update()
    {
        if (_unit != null && _unit.Target == null && Island != null)
        {
            var position = _unitTransform.position;
            _closestTarget = Island.ClosestTarget(position);

            if (_closestTarget != null)
            {
                var closestPoint = _closestTarget?.Hitbox.ClosestPoint(position) ?? _closestTarget.transform.position;
                _unit.Destination = closestPoint;
            }
            else
            {
                _unit.Destination = _unitTransform.position;
            }
        }
    }
}
