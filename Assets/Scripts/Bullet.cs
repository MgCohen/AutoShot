using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public IntVariable damage;
    public int Penetration;
    public float Speed;
    public Animator anim;

    public bool hit = false;
    private void OnEnable()
    {
        Invoke("Kill", 1f);
    }

    private void Update()
    {
        if (!hit)
            transform.position += transform.right * Time.deltaTime * Speed;
    }

    private void Kill()
    {
        if (!hit)
        {
            hit = true;
            anim.SetTrigger("Hit");
            InstancedAction.DelayAction(() => { gameObject.Kill(); }, 0.3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<Enemy>())
        {
            collision.transform.GetComponent<Enemy>().TakeDamage(damage);
        }
        else if (collision.transform.GetComponent<Player>())
        {
            Player.instance.TakeDamage(transform.position, damage);
        }
        Kill();
    }
}
