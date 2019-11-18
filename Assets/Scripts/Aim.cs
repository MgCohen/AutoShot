using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Aim : MonoBehaviour
{
    public Transform target;

    public SpriteRenderer sprite;
    public void Setup(Transform newTarget ,Sprite aimSprite)
    {
        sprite = gameObject.AddComponent<SpriteRenderer>();
        sprite.sprite = aimSprite;
        target = newTarget;
        Set();
    }

    private void Update()
    {
        if(target != null)
        {
            transform.position = target.position;
        }
    }

    public void Set()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 2f).SetEase(Ease.OutBack);
        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }

    public void Unset()
    {
        transform.DOScale(0, 0.5f);
        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).OnComplete(() => { Destroy(gameObject); }).SetEase(Ease.Linear);
    }
}
