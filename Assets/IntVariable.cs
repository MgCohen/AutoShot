using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Int", menuName = "Variables/Int")]
public class IntVariable : ScriptableObject
{
    public int value;

    public static implicit operator int(IntVariable i) => i.value;

    public void Set(int newValue)
    {
        value = newValue;
    }

    public void Set(string newValue)
    {
        int newNumber;
        if (int.TryParse(newValue, out newNumber))
        {
            value = newNumber;
        }
    }
}
