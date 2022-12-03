using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _max;

    private int _current;

    public Collider Hitbox { get; private set; }
    public int Max => _max;
    public int Current
    {
        get => _current;
        set
        {
            _current = value;
            HealthChanged?.Invoke(Current, Max);

            if (Current <= 0)
            {
                Dead?.Invoke(this);
            }
        }
    }

    public event Action<int, int> HealthChanged;
    public event Action<Health> Dead;

    private void Start()
    {
        Current = _max;
        Hitbox = GetComponent<Collider>();
    }

    public void TakeDamage(int damage)
    {
        Current -= Mathf.Min(damage, Current);
    }

    public void FullRecover()
    {
        Current = _max;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
