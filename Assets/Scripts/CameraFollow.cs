using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public BoxCollider2D bound;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = target.position;
        Vector3 currentPos = transform.position;
        currentPos.x = pos.x;
        currentPos.y = pos.y;

        var size = CameraFrame.Frame / 2;
        var limits = bound.size/2 - size;

        currentPos.x = Mathf.Clamp(currentPos.x, -limits.x, limits.x);
        currentPos.y = Mathf.Clamp(currentPos.y, -limits.y, limits.y);

        transform.position = currentPos;

    }
}
