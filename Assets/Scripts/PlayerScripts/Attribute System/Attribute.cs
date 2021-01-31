using System;
using UnityEngine;

[Serializable]
public class Attribute
{
    [SerializeField] private int value;

    public event Action onValueSet;

    public const int MIN = 0;
    public const int MAX = 25;

    public void SetValue()
    {
        value = Mathf.Clamp(value, MIN, MAX);
        onValueSet?.Invoke();
    }

    public int GetValue() { return value; }
}
