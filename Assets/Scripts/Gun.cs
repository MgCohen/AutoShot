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
    bool reloading = false;
    bool shotting = false;
    float reloadTimer;
    public int bulletsShotted;
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
        if (reloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= weapon.reloadTime)
            {
                reloading = false;
                bulletsShotted = 0;
                reloadTimer = 0;
            }
        }
        timer -= Time.deltaTime;
        if (weapon.autoShot)
        {
            TryShot();
        }
        if (sprite.sprite != weapon.weaponSprite)
        {
            sprite.sprite = weapon.weaponSprite;
        }

        sprite.flipY = (TargetSystem.currentTarget.transform.position.x < player.transform.position.x) ? true : false;
    }

    public void TryShot()
    {
        if (reloading || shotting)
        {
            return;
        }
        else if (bulletsShotted >= weapon.magazineSize)
        {
            reloading = true;
            return;
        }
        if (timer <= 0)
        {
            if (!weapon.ammoPerShot)
            {
                bulletsShotted++;
            }
            Shot(weapon.bulletsPerShot);
            timer = (1 / weapon.fireRate);
        }
    }

    public void Shot(int amount = 1)
    {
        var shotDelay = 1 / weapon.fireRate;
        if (amount > 1)
        {
            shotting = true;
        }
        for (int i = 0; i < amount; i++)
        {
            Invoke("Shot", shotDelay * i);
            if (i == amount - 1 && shotting)
            {
                Invoke("EndShot", shotDelay * (i + 1));
            }
        }
        //SimpleCameraShakeInCinemachine.instance.SetShake(0.05f, 0.05f, 1f);
    }

    bool blocked = false;
    public void Shot()
    {
        if (!player.ready && shotting)
        {
            blocked = true;
        }
        if (blocked)
        {
            return;
        }
        if (weapon.ammoPerShot)
            bulletsShotted++;
        if (bulletsShotted >= weapon.magazineSize) reloading = true;
        var deviation = Random.Range(-weapon.accuracy, weapon.accuracy);
        var euler = transform.rotation.eulerAngles;
        euler += new Vector3(0, 0, deviation);
        Instantiate(weapon.bullet, gunPoint.position, Quaternion.Euler(euler));
        var obj = Instantiate(weapon.muzzle, gunPoint.position, Quaternion.Euler(euler));
        Destroy(obj, 0.2f);
        Camera.main.DOShakePosition(0.05f, 0.05f);
    }

    public void EndShot()
    {
        shotting = false;
        blocked = false;
    }
}
