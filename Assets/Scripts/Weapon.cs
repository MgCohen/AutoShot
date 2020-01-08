using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public FloatVariable maxRange;
    public FloatVariable minRange;
    public FloatVariable accuracy;
    public FloatVariable fireRate;

    public IntVariable magazineSize;
    public IntVariable bulletsPerShot;

    public FloatVariable reloadTime;

    public BoolVariable autoShot;
    public BoolVariable ammoPerShot;

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
