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
    public FloatVariable energyRecover;
    public float moveSpeed;
    public float dashSpeed;
    public FloatVariable dashCost;
    public float invunerableTime;

    public int Health;

    public bool ready = true;
    public bool locked = false;
    public bool invunerable = false;

    private Vector2 dashVelocity = new Vector2();
    private Vector2 walkVelocity = new Vector2();
    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += Dash;
        LeanTouch.OnFingerTap += Shot;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= Dash;
        LeanTouch.OnFingerTap -= Shot;
    }

    public void Shot(LeanFinger finger)
    {
        if(!gun.weapon.autoShot)
        gun.TryShot();
    }

    private void Update()
    {
        if (!ready || locked)
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
            float rot = (sprite.flipX) ? 180 : 0;
            gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
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
        if (invunerable)
        {
            return;
        }
        if (collision.gameObject.GetComponent<Enemy>())
        {
            var point = collision.GetContact(0).point;
            TakeDamage(point);
        }
    }

    public void Invunerable()
    {
        invunerable = true;
        InstancedAction.DelayAction(() =>
        {
            invunerable = false;
        }, invunerableTime);
    }

    public void TakeDamage(Vector2 sourcePoint, int amount = 1)
    {
        var direction = (transform.position - (Vector3)sourcePoint).normalized;
        ready = false;
        anim.SetTrigger("Hit");
        Invunerable();
        Health -= amount;
        body.velocity = Vector2.zero;
        if (dashTween != null)
        {
            dashTween.Kill(true);
        }
        body.DOKill();

        body.AddForce(direction * dashSpeed * 350, ForceMode2D.Force);
        InstancedAction.DelayAction(() =>
        {
            ready = true;
            if (Health <= 0)
            {
                Manager.instance.LoseEvent.Raise();
                gameObject.SetActive(false);
            }
        }, 0.25f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gun.weapon.maxRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gun.weapon.minRange);
    }
}