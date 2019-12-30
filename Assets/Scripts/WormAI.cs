using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WormAI : MonoBehaviour
{



    public Enemy enemy;
    public SpriteRenderer sprite;
    public float stalkRange;
    public EnemyBehaviour behaviour;

    public PathMaker path;

    public void Update()
    {
        var playerPos = Player.instance.transform.position;
        var playerDist = (playerPos - transform.position).magnitude;
        var localMask = LayerMask.GetMask("Props");
        var LoS = !Physics2D.Raycast(transform.position, playerPos - transform.position, playerDist, localMask);
        if (playerDist <= stalkRange && LoS)
        {
            behaviour = EnemyBehaviour.Agressive;
            path.speed = 3000;
            path.SetTarget(Player.instance.transform.position);
        }
        else if ((!LoS && playerDist <= stalkRange * 1.25f) || (LoS && playerDist <= stalkRange * 1.5f))
        {
            behaviour = EnemyBehaviour.Near;
            path.speed = 2000;
            path.SetTarget(path.GetPointNearTarget(Player.instance.transform, stalkRange));
        }
        else
        {
            if (behaviour != EnemyBehaviour.Idle || (behaviour == EnemyBehaviour.Idle && path.reachedEnd))
                path.SetTarget(path.GetPointNearTarget(transform, stalkRange));
            behaviour = EnemyBehaviour.Idle;
            path.speed = 1000;
        }

        sprite.flipX = (path.currentVelocity.x < 0);
    }


}
