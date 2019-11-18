using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawner : MonoBehaviour
{
    public GameObject worm;
    public float rate;
    void Start()
    {
        InvokeRepeating("Spawn", rate, rate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        var point = CameraFrame.RandomPoint();
        int counter = 0;
        LayerMask mask = LayerMask.GetMask("Props", "Enemy", "Player");
        while(Physics2D.OverlapCircle(point, 1f, mask))
        {
            counter++;
            if (counter > 500)
            {
                Debug.Log("OUT 2");
                break;
            }
            point = CameraFrame.RandomPoint();
        }
        Instantiate(worm, point, transform.rotation);
    }
}
