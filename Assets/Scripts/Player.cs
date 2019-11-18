using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lean.Touch;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    [Header("References")]
    public Gun gun;
    public SpriteRenderer sprite;
    public Animator anim;
    public Rigidbody2D body;
    public Energy energy;
    public Collider2D coll;

    [Header("Values")]
    public float energyRecover;
    public float moveSpeed;
    public float dashSpeed;
    public float dashCost;

    public int Health;

    public bool ready = true;

    private Vector2 dashVelocity = new Vector2();
    private Vector2 walkVelocity = new Vector2();
    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += Dash;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= Dash;
    }

    private void Update()
    {
        if (!ready)
        {
            return;
        }

        TargetSystem.LockTarget(transform.position, gun.weapon.maxRange);
        if (TargetSystem.currentTarget != null)
        {
            LookAt(TargetSystem.currentTarget.transform.position);
            sprite.flipX = (TargetSystem.currentTarget.transform.position.x < transform.position.x) ? true : false;
        }
        else
        {
            walkVelocity = Vector2.zero;
        }

        body.velocity = walkVelocity + dashVelocity;
        anim.SetBool("Moving", (body.velocity.magnitude > 0.35f));
    }


    public void LookAt(Vector3 pos)
    {
        var dif = pos - transform.position;
        float rotZ = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
        var distance = dif.magnitude;
        if (distance > gun.weapon.maxRange || distance < gun.weapon.minRange)
        {
            MoveToRange(pos);
        }
        else
        {
            walkVelocity = Vector2.zero;
        }

    }

    public void MoveToRange(Vector3 pos)
    {
        var point = pos + ((transform.position - pos).normalized * gun.weapon.averageRange);
        var path = point - transform.position;
        var fixedPath = path;
        if (path.magnitude > 0.5f)
        {
            if (fixedPath.magnitude > 1f) fixedPath = fixedPath.normalized;
            walkVelocity = fixedPath * moveSpeed;
        }
    }

    private Tween dashTween;
    public void Dash(LeanFinger finger)
    {
        if (!ready)
        {
            return;
        }

        if (!energy.Spend(dashCost))
        {
            return;
        }

        body.DOKill();
        if (dashTween != null)
        {
            dashTween.Kill();
        }
        ready = false;
        var dir = finger.SwipeScaledDelta;
        Vector3 dash = dir.normalized * dashSpeed;
        var targetPos = transform.position + dash;
        anim.SetTrigger("Dash");
        sprite.flipX = (targetPos.x < transform.position.x) ? true : false;
        if (sprite.flipX)
        {
            gun.sprite.flipY = true;
            gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Extensions.RotationZ(transform.position, transform.position + dash)));
        }
        body.DOMove(transform.position + dash, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            ready = true;
        });
        dashVelocity = dash + (Vector3)(dir.normalized * moveSpeed);
        dashTween = DOTween.To(() => dashVelocity, x => dashVelocity = x, Vector2.zero, 1f).SetEase(Ease.Linear);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            var point = collision.GetContact(0).point;
            var direction = (transform.position - (Vector3)point).normalized;
            ready = false;
            anim.SetTrigger("Hit");
            coll.enabled = false;
            Health -= 1;
            body.velocity = Vector2.zero;
            if (dashTween != null)
            {
                dashTween.Kill(true);
            }
            body.DOKill();
            body.DOMove(transform.position + direction, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
            {
                coll.enabled = true;
                ready = true;
                if (Health <= 0)
                {
                    Manager.instance.LoseEvent.Raise();
                    gameObject.SetActive(false);
                }
            });
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gun.weapon.maxRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gun.weapon.minRange);
    }
}
