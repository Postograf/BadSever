using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public enum ZoneState
{
    Dynamic,
    Static
}

public class AggressionZone : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private LayerMask _enemiesMask;
    [SerializeField] private ZoneState _state;
    [SerializeField] private float _staticRange;
    [SerializeField] private float _dynamicRange;
    [SerializeField] private ZoneIndicator _indicator;

    private Transform _unitTransform;
    private Vector3 _center;
    private float _range;

    public float StaticRange => _staticRange;
    public float DynamicRange => _dynamicRange;

    public float Range 
    { 
        get => _range;
        private set
        {
            _range = value;
            _indicator?.CalculteLine(_range, _state == ZoneState.Static, Center);
        }
    }

    public Vector3 Center => _state switch
    {
        ZoneState.Dynamic => _unitTransform.position,
        _ => _center
    };

    private void Start()
    {
        _unitTransform = _unit?.transform ?? transform;
        SetState(_state, _unitTransform.position);
    }

    private void Update()
    {
        if (_unit != null)
        {
            if (_unit.Target == null)
            {
                if (TryFindTarget(out var enemy))
                    _unit.Target = enemy;
                else
                    _unit.Destination = Center;
            }
            else
            {
                var sqrDistance = Ruler.SqrDistance(_unit.Target.Hitbox, Center);
                var sqrRange = Range * Range;
                if (sqrDistance > sqrRange)
                {
                    _unit.Target = null;
                    _unit.Destination = Center;
                }
            }
        }
    }

    private bool TryFindTarget(out Health enemy)
    {
        enemy = Physics
            .OverlapSphere(Center, Range, _enemiesMask)
            .Select(c => c.GetComponent<Health>())
            .Where(h => h != null && h.Current > 0)
            .OrderBy(h => Ruler.SqrDistance(h.Hitbox, Center))
            .FirstOrDefault();

        return enemy != null;
    }

    public void SetState(ZoneState state, Vector3? center = null)
    {
        _state = state;

        if (_state == ZoneState.Static)
        {
            _center = center.HasValue ? center.Value : _center;
            Range = _staticRange;
        }
        else
        {
            Range = _dynamicRange;
        }
    }

    public void Show(bool state)
    {
        _indicator?.Show(state);
    }
}
