using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public float maxEnergy;
    public float energyAmount;

    public Slider slider;
    private void OnEnable()
    {
        slider.maxValue = maxEnergy;
        slider.value = maxEnergy;
        energyAmount = maxEnergy;
    }
    public bool Spend(float amount)
    {
        if(amount> energyAmount)
        {
            return false;
        }
        energyAmount -= amount;
        slider.value = energyAmount;
        return true;
    }

    private void Update()
    {
        energyAmount += Player.instance.energyRecover * Time.deltaTime;
        if(energyAmount > maxEnergy)
        {
            energyAmount = maxEnergy;
        }
        slider.value = energyAmount;
    }
}
