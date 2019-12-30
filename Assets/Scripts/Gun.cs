using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour
{


    public Transform gunPoint;
    public Player player;
    public SpriteRenderer sprite;
    public Weapon weapon;

    public float timer;

    private void Start()
    {
        timer = 1 / weapon.fireRate;
    }
    private void Update()
    {
        if (!player.ready || player.locked)
        {
            return;
        }
        if (TargetSystem.currentTarget == null)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Shot();
            timer = (1 / weapon.fireRate);
        }
        if (sprite.sprite != weapon.weaponSprite)
        {
            sprite.sprite =  weapon.weaponSprite;
        }

        sprite.flipY = (TargetSystem.currentTarget.transform.position.x < player.transform.position.x)? true : false;
    }

    public void Shot()
    {
        var deviation = Random.Range(-weapon.accuracy, weapon.accuracy);
        var euler = transform.rotation.eulerAngles;
        euler += new Vector3(0, 0, deviation);
        Instantiate(weapon.bullet, gunPoint.position, Quaternion.Euler(euler));
        var obj = Instantiate(weapon.muzzle, gunPoint.position, Quaternion.Euler(euler));
        Destroy(obj, 0.2f);
        Camera.main.DOShakePosition(0.05f, 0.05f);
        //SimpleCameraShakeInCinemachine.instance.SetShake(0.05f, 0.05f, 1f);
    }
}
