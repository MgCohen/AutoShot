using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Enemy : MonoBehaviour, ITarget
{

    public static List<Enemy> currentEnemies = new List<Enemy>();


    public bool immune = false;
    public int health;
    public int Endurance;
    public bool dead = false;
    public bool hurt = false;
    public Animator anim;
    public Collider2D coll;
    public Rigidbody2D rb;

    public GameEvent win;

    private void OnEnable()
    {
        currentEnemies.Add(this);
    }

    private void OnDisable()
    {
        currentEnemies.Remove(this);
        if(currentEnemies.Count <= 0)
        {
            win.Raise();
        }
    }

    public void SetTarget()
    {
        TargetSystem.SetTarget(gameObject);
    }

    private void Update()
    {
        if (dead)
        {
            rb.velocity = Vector2.zero;
            coll.enabled = false;
        }
    }

    public void TakeDamage(int amount, int Penetration = 0)
    {
        if (immune)
        {
            return;
        }
        if(amount <= 0)
        {
            return;
        }
        if (Penetration >= Endurance)
        {
            anim.SetTrigger("Hurt");
        }
        health -= amount;
        if (health <= 0)
        {
            if(Penetration < Endurance)
            anim.SetTrigger("Hurt");
            anim.SetBool("Dead", true);
            coll.enabled = false;
            rb.velocity = Vector2.zero;
            GetComponent<PathMaker>().stopped = true;
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
