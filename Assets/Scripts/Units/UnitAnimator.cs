using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(Unit), typeof(NavMeshAgent))]
public class UnitAnimator : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private string _moveSpeedProperty = "MoveSpeed";
    [SerializeField] private float _standartMoveSpeed;

    [Header("Attack")]
    [SerializeField] private string _attackSpeedProperty = "AttackSpeed";
    [SerializeField] private string _isAttackProperty = "IsAttack";
    [SerializeField] private float _standartAttackSpeed;

    [Header("Death")]
    [SerializeField] private string _isDeadProperty = "IsDead";

    private Animator _animator;
    private Unit _unit;
    private NavMeshAgent _agent;
    private Health _health;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _unit = GetComponent<Unit>();

        if (TryGetComponent(out _health)) 
            _health.Dead += Dead;
    }

    private void Update()
    {
        if (_agent?.hasPath ?? false)
        {
            var unitSpeed = _agent?.velocity.magnitude ?? 0;
            _animator.SetFloat(_moveSpeedProperty, unitSpeed / _standartMoveSpeed);
        }
        else
        {
            _animator.SetFloat(_moveSpeedProperty, 0);
        }
    }

    public void Attack(bool state)
    {
        var unitAttackSpeed = _unit?.AttackSpeed ?? 0;
        _animator?.SetFloat(_attackSpeedProperty, unitAttackSpeed / _standartAttackSpeed);
        _animator?.SetBool(_isAttackProperty, state);
    }

    public void Dead(Health health)
    {
        _animator.SetFloat(_moveSpeedProperty, 0);
        _animator.SetBool(_isAttackProperty, false);
        _animator.SetBool(_isDeadProperty, true);
    }
}
