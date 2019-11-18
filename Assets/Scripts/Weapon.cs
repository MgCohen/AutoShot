using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public float maxRange;
    public float minRange;
    public float accuracy;
    public float fireRate;
    public GameObject bullet;
    public GameObject muzzle;

    public Sprite weaponSprite;
    public Sprite AimSprite;
    public float averageRange
    {
        get
        {
            return minRange + ((maxRange - minRange) / 2);
        }
    }

}
