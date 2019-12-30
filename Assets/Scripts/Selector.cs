using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Selector : MonoBehaviour
{

    public float radius;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += Select;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= Select;
    }

    public void Select(LeanFinger finger)
    {
        var point = finger.GetWorldPosition(0);
        point.z = 0;
        var localMask = LayerMask.GetMask("Enemy");
        var hits = Physics2D.OverlapCircleAll(point, radius, localMask);
        GameObject target = null;
        float distance = 999f;
        foreach (var hit in hits)
        {
            var targettable = hit.GetComponent<ITarget>();
            if(targettable != null)
            {
                var dist = (point - hit.transform.position).magnitude;
                if(dist < distance)
                {
                    target = hit.gameObject;
                }
            }
        }
        if(target != null)
        {
            target.GetComponent<ITarget>().SetTarget();
        }
    }


}
