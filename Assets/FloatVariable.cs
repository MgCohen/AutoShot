using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Float", menuName = "Variables/Float")]
public class FloatVariable : ScriptableObject
{
    public float value;

    public static implicit operator float(FloatVariable f) => f.value;

    public void Set(float newValue)
    {
        value = newValue;
    }

    public void Set(string newValue)
    {
        float newNumber;
        if (float.TryParse(newValue, out newNumber))
        {
            value = newNumber;
        }
    }
}
