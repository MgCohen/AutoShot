using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bool", menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject
{
    public bool value;

    public static implicit operator bool(BoolVariable b) => b.value;

    public void Set(bool newValue)
    {
        value = newValue;
    }
}
