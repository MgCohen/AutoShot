using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class TargetSystem
{
    public static GameObject currentTarget = null;
    public static Aim currentAim;

    public static UnityEvent OnTargetChange = new UnityEvent();
    public static UnityEvent OnSetTarget = new UnityEvent();

    public static void SetTarget(GameObject target)
    {
        if (target == currentTarget || target == null)
        {
            return;
        }
        if(currentTarget != null)
        {
            OnTargetChange.Invoke();
            DumpTarget();
        }
        currentTarget = target;
        SetTarget();
        OnSetTarget.Invoke();
    }

    public static void LockTarget(Vector3 Pos, float radius, GameObject source = null)
    {
        if(currentTarget != null && source == null)
        {
            return;
        }

        var mask = LayerMask.GetMask("Enemy");
        var hits = Physics2D.OverlapCircleAll(Pos, radius, mask);
        if (hits.Length > 0)
        {
            GameObject target = null;
            float distance = 999f;
            foreach (var hit in hits)
            {
                if (hit.GetComponent<ITarget>() == null || (hit.GetComponent<Enemy>() != null && hit.GetComponent<Enemy>().dead) ||source == hit.gameObject)
                {
                    continue;
                }
                var tryDistance = (hit.transform.position - Pos).magnitude;
                var localMask = LayerMask.GetMask("Props");
                var LoS = !Physics2D.Raycast(Pos, hit.transform.position - Pos, tryDistance, localMask);
                var playerPos = Player.instance.transform.position;
                var playerDist = (playerPos - hit.transform.position).magnitude;
                var pLos = !Physics2D.Raycast(playerPos, hit.transform.position - playerPos, playerDist, localMask);
                if(tryDistance < distance && pLos)
                {
                    target = hit.gameObject;
                    distance = tryDistance;
                }
            }
            if(target != null)
            {
                SetTarget(target);
            }
        }
    }

    public static void SetTarget()
    {
        var newAim = new GameObject("Aim").AddComponent<Aim>();
        newAim.Setup(currentTarget.transform, Player.instance.gun.weapon.AimSprite);
        currentAim = newAim;
    }

    public static void DumpTarget(bool needTarget = false)
    {
        var oldTarget = currentTarget;
        currentAim.Unset();
        currentAim = null;
        currentTarget = null;
        if (needTarget)
        {
            LockTarget(oldTarget.transform.position, 15f, oldTarget);
        }
    }

}
