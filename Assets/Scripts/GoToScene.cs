using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GoToScene : MonoBehaviour
{

    public bool fadeIn;
    public Image image;
    public float fadeTime = 1;
    public int index;

    public void Change()
    {
        if (!fadeIn)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }
        else
        {
            image.gameObject.SetActive(true);
            image.DOFade(1, fadeTime);
            InstancedAction.DelayAction(() => UnityEngine.SceneManagement.SceneManager.LoadScene(index), fadeTime);
        }
    }
}
