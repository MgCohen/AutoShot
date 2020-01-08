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

    public GameEvent winEvent;
    public GameObject winPanel;
    public GameObject finalPanel;
    private void OnEnable()
    {
        LoseEvent.Register(Lose);
        winEvent.Register(Win);
    }

    private void OnDisable()
    {
        LoseEvent.Unregister(Lose);
        winEvent.Unregister(Win);
    }
    public void Lose()
    {
        losePanel.SetActive(true);
    }

    public void Win()
    {
        var count = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        var index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if(index >= count - 1)
        {
            finalPanel.SetActive(true);
        }
        else
        {
            winPanel.SetActive(true);
        }
    }

    public void Pause()
    {
        ClockSystem.Stop();
    }

    public void UnPause()
    {
        ClockSystem.Resume();
    }

    public void ChangeAttackType()
    {

    }

}
