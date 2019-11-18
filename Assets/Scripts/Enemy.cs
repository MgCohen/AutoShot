using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Enemy : MonoBehaviour, ITarget
{
    public int health;
    public int Endurance;
    public bool dead = false;
    public bool hurt = false;
    public Animator anim;
    public Collider2D coll;

    public void SetTarget()
    {
        TargetSystem.SetTarget(gameObject);
    }

    public void TakeDamage(int amount, int Penetration = 0)
    {
        if (Penetration >= Endurance)
        {
            anim.SetTrigger("Hurt");
        }
        health -= amount;
        if (health <= 0)
        {
            anim.SetTrigger("Hurt");
            anim.SetBool("Dead", true);
            coll.enabled = false;
            if (TargetSystem.currentTarget == gameObject)
            {
                TargetSystem.DumpTarget(true);
            }
            ClockSystem.ChangeScale(0.1f, 0.1f);
            Camera.main.DOShakePosition(0.25f, 0.75f);
            //SimpleCameraShakeInCinemachine.instance.SetShake(0.25f, 0.75f, 1f);
            Destroy(gameObject, 2f);
            dead = true;

        }
    }
}
