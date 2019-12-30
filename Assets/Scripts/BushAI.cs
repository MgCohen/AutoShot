using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;

public class BushAI : MonoBehaviour
{
    [Header("References")]
    public PathMaker path;
    public SpriteRenderer sprite;
    public Animator anim;
    public Enemy enemy;

    bool checking = false;
    float timer = 0;

    
    [Header("Values")]
    public EnemyBehaviour behaviour = EnemyBehaviour.Idle;
    public bool striked = false;
    public float checkTime;
    public float recoverTime;
    public float circlingRange;
    public float NearRange;
    public float IdleRange;
    public float strikeForce;



    Vector2 strikeDirection = new Vector2();

    private void Update()
    {
        if (enemy.dead)
        {
            return;
        }
        var moving = (path.rb.velocity.magnitude > 0.2f) ? true : false;
        anim.SetBool("Moving", moving);
        sprite.flipX = (path.currentVelocity.x < 0);
        SetBehaviour();
    }


    public void SetBehaviour()
    {
        var playerPos = Player.instance.transform.position;
        var playerDist = (playerPos - transform.position).magnitude;
        var mask = LayerMask.GetMask("Props");
        var playerSight = !Physics2D.Raycast(transform.position, playerPos - transform.position, playerDist, mask);

        if (playerDist < circlingRange && playerSight && !striked)
        {
            behaviour = EnemyBehaviour.Agressive;
            if (!checking)
            {
                checking = true;
                if (timer < checkTime)
                    timer = checkTime;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0 && checking)
                {
                    if (playerDist <= circlingRange * 1.2f)
                    {
                        Debug.Log("STRIKE");
                        striked = true;
                        path.stopped = true;
                        path.rb.velocity = Vector2.zero; 
                        anim.SetTrigger("Strike");
                        strikeDirection = (playerPos - transform.position).normalized;
                    }
                    else
                    {
                        checking = false;
                    }
                }
            }
        }

        if ((playerDist < NearRange * 0.75 && !playerSight) || (playerDist < NearRange && playerSight))
        {
            if (behaviour != EnemyBehaviour.Near) path.GetRandomOffset();
            behaviour = EnemyBehaviour.Near;
            path.SetTarget(path.GetPointOnRandomDirection(Player.instance.transform, 10f, circlingRange));
        }
        else
        {
            if ((behaviour == EnemyBehaviour.Idle && path.reachedEnd) || behaviour != EnemyBehaviour.Idle)
            {
                var chance = Random.value;
                if (chance > 0.3f)
                {
                    path.SetTarget(path.GetPointNearTarget(transform, IdleRange));
                }
                else
                {
                    path.stopped = true;
                    InstancedAction.DelayAction(() => { path.stopped = false; }, 1f);
                }
            }
            behaviour = EnemyBehaviour.Idle;

        }
    }


    public void Dash()
    {
        if (enemy.dead)
        {
            return;
        }
        enemy.immune = true;
        //path.rb.DOMove((Vector2)transform.position + (strikeDirection * force), 0.5f);
        Debug.Log(strikeDirection);
        path.rb.AddForce(strikeDirection * strikeForce, ForceMode2D.Impulse);
        InstancedAction.DelayAction(() =>
        {
            path.rb.velocity = Vector2.zero;
        }, 0.5f);
        InstancedAction.DelayAction(() =>
        {
            enemy.immune = false;
            anim.SetTrigger("Up");
        }, 1f);
    }

    public void Up()
    {
        Debug.Log("Up");
        path.stopped = false;
        striked = false;
        checking = false;
        timer = recoverTime;
        behaviour = EnemyBehaviour.Idle;
    }
}
