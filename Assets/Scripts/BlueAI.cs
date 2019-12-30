using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueAI : MonoBehaviour
{

    [Header("References")]
    public PathMaker path;
    public SpriteRenderer sprite;
    public Animator anim;
    public Enemy enemy;

    [Header("Values")]
    public EnemyBehaviour behaviour = EnemyBehaviour.Idle;
    EnemyBehaviour lastBehaviour = EnemyBehaviour.Idle;
    public float shotRange;
    public float chaseRange;
    public float idleRange;
    public bool idling;

    public float shotCd;
    public GameObject bullet;
    public Transform bulletPoint;
    bool shoting = false;
    float timer;
    Vector2 shotDirection;

    private void OnEnable()
    {
        timer = shotCd / 2;
    }
    private void Update()
    {
        if (enemy.dead)
        {
            return;
        }
        if (timer > 0) timer -= Time.deltaTime;
        var moving = (path.rb.velocity.magnitude > 0.2f) ? true : false;
        anim.SetBool("Moving", moving);
        if (!shoting)
        {
            sprite.flipX = (path.currentVelocity.x < 0);
        }
        else
        {
            var dir = Player.instance.transform.position - transform.position;
            sprite.flipX = (dir.x < 0);
        }
        var point = bulletPoint.localPosition;
        point.x = (sprite.flipX) ? -(Mathf.Abs(point.x)) : (Mathf.Abs(point.x));
        bulletPoint.localPosition = point;
        SetBehaviour();
    }

    public void SetBehaviour()
    {
        var playerPos = Player.instance.transform.position;
        var playerDist = (playerPos - transform.position).magnitude;
        var mask = LayerMask.GetMask("Props");
        var playerSight = !Physics2D.Raycast(transform.position, playerPos - transform.position, playerDist, mask);
        lastBehaviour = behaviour;
        if (playerDist <= shotRange && playerSight)
        {
            behaviour = EnemyBehaviour.Agressive;
        }
        else if (playerDist <= chaseRange)
        {
            behaviour = EnemyBehaviour.Near;
        }
        else
        {
            behaviour = EnemyBehaviour.Idle;
        }

        HandleBehaviour();
    }

    public void HandleBehaviour()
    {
        if (idling)
        {
            return;
        }
        if (behaviour == EnemyBehaviour.Idle)
        {
            if (path.reachedEnd)
            {
                var chance = Random.value;
                Debug.Log(chance);
                if (chance <= 0.1f)
                {
                    idling = true;
                    path.reachedEnd = false;
                    path.NullTarget();
                    InstancedAction.DelayAction(() =>
                    {
                        idling = false;
                        path.reachedEnd = true;
                    }, 1f);
                }
                else
                {
                    path.SetTarget(path.GetPointNearTarget(transform, idleRange));
                }
            }
        }
        else if (behaviour == EnemyBehaviour.Near)
        {
            if (path.reachedEnd)
            {
                path.SetTarget(path.GetPointOnDirection(Player.instance.transform, 20f, shotRange, (transform.position - Player.instance.transform.position)));
            }
        }
        else
        {
            if (lastBehaviour != EnemyBehaviour.Agressive)
            {
                path.SetTarget(path.GetPointOnDirection(Player.instance.transform, 20f, shotRange, (transform.position - Player.instance.transform.position)));
            }
            else if(path.reachedEnd)
            {
                var offset = Random.Range(15, 80);
                path.SetTarget(path.GetPointOnDirection(Player.instance.transform, offset, shotRange, (transform.position - Player.instance.transform.position), true));
            }
            if (path.targetDistance <= shotRange * 1.2f && timer <= 0)
            {

                var direction = Player.instance.transform.position - bulletPoint.position;
                sprite.flipX = (direction.x < 0);
                anim.SetTrigger("Shot");
                timer = shotCd;
                idling = true;
                shoting = true;
                shotDirection = direction.normalized;
                path.reachedEnd = false;
                path.NullTarget();
                InstancedAction.DelayAction(() =>
                {
                    idling = false;
                    path.reachedEnd = true;
                }, 1f);
            }
        }
    }

    public void Shot()
    {
        shoting = false;
        //var rotz = Extensions.RotationZ(transform.position, shotDirection);
        var rotZ = Extensions.RotationZ(bulletPoint.position, Player.instance.transform.position);
        Instantiate(bullet, bulletPoint.position, Quaternion.Euler(new Vector3(0, 0, rotZ)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shotRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
