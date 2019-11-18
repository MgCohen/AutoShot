using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }


    public GameEvent LoseEvent;
    public GameObject losePanel;


    private void OnEnable()
    {
        LoseEvent.Register(Lose);
    }

    private void OnDisable()
    {
        LoseEvent.Unregister(Lose);
    }
    public void Lose()
    {
        losePanel.SetActive(true);
    }
}
