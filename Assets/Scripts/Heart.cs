using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int index;

    public GameObject heart;

    private void Update()
    {
        heart.SetActive((Player.instance.Health >= index));
    }
}
