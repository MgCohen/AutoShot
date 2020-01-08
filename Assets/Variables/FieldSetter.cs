using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldSetter : MonoBehaviour
{
    public FloatVariable myFloat;
    public IntVariable myInt;
    public BoolVariable myBool;

    TMP_InputField field;
    Toggle toggle;
    private void OnEnable()
    {
        field = GetComponent<TMP_InputField>();
        toggle = GetComponent<Toggle>();
        if (field)
        {
            if (myFloat)
            {
                //field.text = myFloat.ToString();
                field.SetTextWithoutNotify(myFloat.value.ToString());
                Debug.Log(1);
            }
            else if (myInt)
            {
                //field.text = myInt.ToString();
                field.SetTextWithoutNotify(myInt.value.ToString());
            }
        }
        else if (toggle)
        {
            if (myBool)
            {
                toggle.isOn = myBool;
            }
        }

    }
}
