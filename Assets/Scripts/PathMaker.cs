using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public enum EnemyBehaviour { Idle, Near, Agressive };

public class PathMaker : MonoBehaviour
{

    public bool stopped = false;

    public Vector2 target;
    public float targetDistance;
    bool hasTarget;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public Vector2 currentVelocity;

    Path path;
    int currentWaypoint;
    public bool reachedEnd = true;

    public Seeker seeker;
    public Rigidbody2D rb;


    private bool pathPending = true;
    private bool agentReady
    {
        get
        {
            return (pathPending && reachedEnd);
        }
    }


    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            target = Player.instance.transform.position;
        }
        InvokeRepeating("GetPath", 0f, 0.5f);
    }

    public void GetPath()
    {
        if (target == null || !hasTarget)
        {
            path = null;
            return;
        }
        seeker.StartPath(rb.position, target, UpdatePath);
        pathPending = true;
    }

    public void UpdatePath(Path p)
    {
        if (!p.error)
        {
            pathPending = false;
            path = p;
            currentWaypoint = 0;
        }
    }

    public void SetTarget(Vector3 position)
    {
        target = position;
        hasTarget = true;
    }

    public void NullTarget()
    {
        hasTarget = false;
    }

    private void FixedUpdate()
    {
        if (path == null || stopped)
        {
            return;
        }
        reachedEnd = (currentWaypoint >= path.vectorPath.Count) ? true : false;
        if (reachedEnd) return;
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        currentVelocity = rb.velocity;
        targetDistance = (target - (Vector2)transform.position).magnitude;
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    private Vector2 GetPointNearTargetRandom(Transform mtarget, float distance)
    {
        var point = Random.insideUnitCircle * distance;
        point += (Vector2)mtarget.position;
        return point;
    }

    public Vector2 GetPointNearTarget(Transform mtarget, float distance)
    {
        var point = GetPointNearTargetRandom(mtarget, distance);
        var node1 = AstarPath.active.GetNearest(point).node;
        var node2 = AstarPath.active.GetNearest(transform.position).node;
        int counter = 0;
        while (!PathUtilities.IsPathPossible(node1, node2))
        {
            counter++;
            if (counter > 500)
            {
                Debug.Log("OUT 2");
                break;
            }
            point = GetPointNearTargetRandom(mtarget, distance);
            node1 = AstarPath.active.GetNearest(point).node;
        }
        return point;
    }

    public Vector2 GetRandomOffset()
    {
        var randomX = Random.Range(-1f, 1f);
        var randomY = Random.Range(-1f, 1f);
        return  new Vector2(randomX, randomY).normalized;
    }

    public Vector2 GetPointOnDirection(Transform mTarget, float variation, float distance, Vector2 direction, bool fixedVariation = false)
    {
        direction = direction.normalized;
        var signed = Vector2.SignedAngle(Vector2.right, direction);
        if (!fixedVariation)
        {
            signed += Random.Range(-variation, +variation);
        }
        else
        {
            var chance = Random.value;
            variation = (chance <= 0.5f) ? -variation : variation;
            signed += variation;
        }
        var pos = (Extensions.DegreeToVector2(signed).normalized * distance) + (Vector2)mTarget.position;
        var node = AstarPath.active.GetNearest(pos).node;
        return (Vector3)node.position;
    }

    public Vector2 GetPointOnRandomDirection(Transform mTarget, float variation, float distance)
    {
        return GetPointOnDirection(mTarget, variation, distance, GetRandomOffset());
    }

    private Vector2 GetPointAroundRandom(Transform mTarget, float radius, float variation = 2f)
    {
        Vector2 point = mTarget.position;
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        var randomDirection = new Vector2(randomX, randomY).normalized;
        variation = Random.Range(-variation, variation);
        point += randomDirection * (radius + variation);
        return point;
    }

    public Vector2 GetPointAround(Transform mTarget, float radius, float variation = 2f)
    {
        var point = GetPointAroundRandom(mTarget, radius, variation);
        var node1 = AstarPath.active.GetNearest(point).node;
        var node2 = AstarPath.active.GetNearest(transform.position).node;
        int counter = 0;
        while (!PathUtilities.IsPathPossible(node1, node2))
        {
            counter++;
            if (counter > 500)
            {
                Debug.Log("OUT 2");
                break;
            }
            point = GetPointAroundRandom(mTarget, radius, variation);
            node1 = AstarPath.active.GetNearest(point).node;
        }
        return point;
    }

}
