using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Boat : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _capacity;

    private List<Unit> _units = new List<Unit>();
    private Collider _collider;
    private Rigidbody _rigidbody;
    private Transform _transform;

    private float _distanceBetweenUnits;
    private Vector3 _freeSpace;

    public int Capacity => _capacity;
    public int UnitsCount => _units.Count;
    public float Speed => _speed;

    public void Init()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;

        var length = _collider.bounds.size.z;
        _distanceBetweenUnits = length / _capacity;
        _freeSpace = Vector3.zero;
        _freeSpace.z += length / 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enabled && collision.gameObject.TryGetComponent(out Island island))
        {
            _rigidbody.velocity *= 0;
            DropUnits();
            enabled = false;
        }
    }

    private void DropUnits()
    {
        foreach (var unit in _units)
        {
            unit.transform.parent = null;
            
            //ѕерезагрузка агента так как создавалс€ не на NavMesh
            //Ќа него они перемест€тс€ автоматически
            unit.Agent.enabled = false;
            unit.Agent.enabled = true;
        }

        _units.Clear();
    }

    public bool TryAddUnit(Unit unit)
    {
        if (_units.Count < _capacity)
        {
            unit.transform.parent = _transform;
            unit.transform.localPosition = _freeSpace;
            _freeSpace.z -= _distanceBetweenUnits;

            _units.Add(unit);
            return true;
        }

        return false;
    }
}