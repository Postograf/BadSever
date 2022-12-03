using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [SerializeField] private PathIndicator _pathIndicator;
    [SerializeField] private AggressionZone _aggressionZone;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _damage;

    private NavMeshAgent _agent;
    private Transform _transform;
    private UnitAnimator _animator;
    private float _sqrAttackRange;
    private Health _target;
    private Health _health;

    private Vector3? _location;

    public bool IsDead { get; private set; }
    public float AttackSpeed => _attackSpeed;
    public NavMeshAgent Agent => _agent;
    public Vector3 Destination { get; set; }
    public Health Target
    {
        get => _target;
        set
        {
            if (IsDead) return;

            _target = value;

            if (value == null)
            {
                _animator?.Attack(false);
            }
            else
            {
                value.Dead += OnTargetDead;
            }
        }
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<UnitAnimator>();
        _transform = transform;

        if (TryGetComponent(out _health))
        {
            _health.Dead += OnDead;
        }

        Destination = _transform.position;
        _sqrAttackRange = _attackRange * _attackRange;
    }

    private void Update()
    {
        if (_agent.isOnNavMesh)
        {
            if (_location.HasValue)
            {
                MoveTo(_location.Value);
                _pathIndicator?.UpdateLinePath(_agent.path.corners);

                var sqrAggressionRange = _aggressionZone.StaticRange * _aggressionZone.StaticRange;
                if ((_agent.destination - _transform.position).sqrMagnitude <= sqrAggressionRange)
                {
                    _location = null;
                    _aggressionZone.SetState(ZoneState.Static, _agent.destination);
                    _pathIndicator?.UpdateLinePath(null);
                }
            }
            else if (Target != null)
            {
                var closestPoint = Target.Hitbox.ClosestPoint(_transform.position);
                MoveTo(closestPoint, _attackRange, closestPoint);
            }
            else
            {
                MoveTo(Destination);
            }
        }

        if (Target != null)
        {
            Attack();
        }
    }

    private void MoveTo(Vector3 destination, float stoppingDistance = 0, Vector3? endPathLookAt = null)
    {
        _agent.stoppingDistance = stoppingDistance;
        _agent.destination = destination;

        var corners = _agent.path.corners;
        if (corners.Length > 1)
            _transform.LookAt(corners[1]);
        else if (endPathLookAt.HasValue)
            _transform.LookAt(endPathLookAt.Value);
    }

    private void Attack()
    {
        var sqrDistance = Ruler.SqrDistance(Target.Hitbox, _transform.position);
        if (sqrDistance < _sqrAttackRange)
        {
            if (_animator != null)
                _animator?.Attack(true);
            else
                DamageTarget();
        }
        else
        {
            _animator?.Attack(false);
        }
    }

    public void Relocate(Vector3 point)
    {
        _location = point;
        Target = null;
        _aggressionZone.SetState(ZoneState.Dynamic);
    }

    //Активируется анимацией
    public void DamageTarget()
    {
        if (Target != null)
        {
            var sqrDistance = Ruler.SqrDistance(Target.Hitbox, _transform.position);

            if (sqrDistance <= _sqrAttackRange)
            {
                Target.TakeDamage(_damage);
            }
        }
    }

    public void Select(bool select)
    {
        _aggressionZone.Show(select && IsDead == false);
    }

    private void OnTargetDead(Health health)
    {
        if (Target != null)
            Target.Dead -= OnTargetDead;
        Target = null;
    }

    private void OnDead(Health health)
    {
        _health.Dead -= OnDead;
        IsDead = true;
        enabled = false;
        _agent.enabled = false;

        _aggressionZone?.Show(false);
        _pathIndicator?.UpdateLinePath(null);
    }
}
