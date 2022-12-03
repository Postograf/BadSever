using System;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private int _unitsCount;
    [SerializeField] private int _preferredBoatsCount;
    [SerializeField] private float _delayBefore;

    public int UnitsCount => _unitsCount;
    public int PreferredBoatsCount => _preferredBoatsCount;
    public float DelayBefore => _delayBefore;
}