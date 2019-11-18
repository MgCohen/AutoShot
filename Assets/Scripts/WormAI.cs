using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WormAI : MonoBehaviour
{

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float stalkRange = 10;

    Path path;
    int currentWaypoint;
    float currentSpeed;
    float targetDistance;
    bool reachedEnd = true;

    public Seeker seeker;
    public Rigidbody2D rb;
    public Enemy enemy;
    public SpriteRenderer sprite;

    private bool pathPending = true;
    private bool agentReady
    {
        get
        {
            return (pathPending && reachedEnd);
        }
    }

    public enum Behaviour { Idle, Near, Agressive };
    public Behaviour behaviour;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        if(target == null)
        {
            target = Player.instance.transform;
        }
        if(enemy == null)
        {
            GetComponent<Enemy>();
        }
        InvokeRepeating("GetPath", 0f, 0.5f);
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

    public void GetPath()
    {

        SetBehaviour();
        if (behaviour == Behaviour.Idle && agentReady)
        {
            currentSpeed = speed;
            var point = GetRandomPointReal();
            seeker.StartPath(rb.position, point, UpdatePath);
        }
        else if (behaviour == Behaviour.Near && (agentReady || (!agentReady && targetDistance > 12)))
        {
            currentSpeed = speed * 1.5f;
            var point = GetPointNearTargetReal();
            seeker.StartPath(rb.position, point, UpdatePath);
        }
        else if (behaviour == Behaviour.Agressive)
        {
            currentSpeed = speed * 2f;
            seeker.StartPath(rb.position, target.position, UpdatePath);
        }
        pathPending = true;
    }

    public void SetBehaviour()
    {
        var distance = (rb.position - (Vector2)target.position).magnitude;
        targetDistance = distance;
        var localMask = LayerMask.GetMask("Props");
        var LoS = !Physics2D.Raycast(rb.position, ((Vector2)target.position - rb.position), distance, localMask);
        if(distance <= 6)
        {
            behaviour = Behaviour.Agressive;
        }
        else if((!LoS && distance <= 8) || (LoS && distance <= 10))
        {
            behaviour = Behaviour.Near;
        }
        else
        {
            behaviour = Behaviour.Idle;
        }
    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (enemy.dead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        reachedEnd = (currentWaypoint >= path.vectorPath.Count) ? true : false;
        if (reachedEnd) return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * currentSpeed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        sprite.flipX = (rb.velocity.x >= 0) ? false : true;
    }

    public Vector2 GetRandomPoint()
    {
        GraphNode randomNode;

        // For grid graphs
        var grid = AstarPath.active.data.gridGraph;

        randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];


        // Use the center of the node as the destination for example
        return (Vector3)randomNode.position;
    }

    public Vector2 GetRandomPointReal()
    {
        var point = GetRandomPoint();
        var node1 = AstarPath.active.GetNearest(point).node;
        var node2 = AstarPath.active.GetNearest(transform.position).node;
        int counter = 0;
        while(!PathUtilities.IsPathPossible(node1, node2))
        {
            counter++;
            if (counter > 500)
            {
                Debug.Log("OUT 2");
                break;
            }
            point = GetRandomPoint();
            node1 = AstarPath.active.GetNearest(point).node;
        }
        return point;
    }

    public Vector2 GetPointNearTarget()
    {
        var point = Random.insideUnitCircle * stalkRange;
        point += (Vector2)target.position;
        return point;
    }

    public Vector2 GetPointNearTargetReal()
    {
        var point = GetPointNearTarget();
        var node1 = AstarPath.active.GetNearest(point).node;
        var node2 = AstarPath.active.GetNearest(transform.position).node;
        int counter = 0;
        while (!PathUtilities.IsPathPossible(node1, node2))
        {
            counter++;
            if(counter > 500)
            {
                Debug.Log("OUT 2");
                break;
            }
            point = GetPointNearTarget();
            node1 = AstarPath.active.GetNearest(point).node;
        }
        return point;
    }
}
