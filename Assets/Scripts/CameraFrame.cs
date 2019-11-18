using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFrame
{

    public static Vector2 Frame
    {
        get
        {
            float height = Camera.main.orthographicSize * 2.0f;
            float width = height * Camera.main.aspect;
            return new Vector2(width, height);
        }
    }

    public static Vector2 RandomPoint()
    {
        var randomX = Random.Range(-Frame.x / 2, Frame.x / 2);
        var randomY = Random.Range(-Frame.y / 2, Frame.y / 2);
        return new Vector2(randomX, randomY);
    }

}
