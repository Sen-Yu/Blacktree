using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : Monster
{
    public enum State
    {
        Idle,
        Run,
        Attack,
    };
    public State currentState = State.Idle;

    public Transform[] wallCheck;
    public Transform genPoint;
    public GameObject Bullet;

    WaitForSeconds Delay1000 = new WaitForSeconds(0.9f);

    void Awake()
    {
        base.Awake();
        moveSpeed = 1f;
        jumpPower = 15f;
        currentHp = 4;
        atkCooltime = 3f;
        atkCooltimeCalc = atkCooltime;

        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while(true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator Idle()
    {
        yield return null;
        MyAnimSetTrigger("Idle");

        if (Random.value > 0.5f)
        {
            MonsterFlip();
        }
        yield return Delay1000;
        currentState = State.Run;
    }

    IEnumerator Run()
    {
        yield return null;
        float runTime = Random.Range(2f, 3f);
        
        while (runTime >= 0f)
        {
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);

            if (Physics2D.OverlapCircle(wallCheck[1].position, 0.05f, layerMask))
            {
                MonsterFlip();
            }

            if(canAtk && IsPlayerDir())
            {
                if(Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < 15f)
                {
                    currentState = State.Attack;
                    break;
                }
            }
            yield return null;
        }
        if (currentState != State.Attack)
        {
            if (Random.value > 0.5f)
            {
                MonsterFlip();
            }
            else
            {
                currentState = State.Idle;
            }
        }
    }

    IEnumerator Attack ()
    {
        yield return null;
        
        canAtk = false;
        rb.velocity = new Vector2(0, jumpPower);
        MyAnimSetTrigger("Attack");
        yield return Delay1000;
        currentState = State.Idle;
    }

    void Fire()
    {
        GameObject bulletClone = Instantiate(Bullet, genPoint.position, transform.rotation);
        bulletClone.GetComponent<Rigidbody2D>().velocity = transform.right * -transform.localScale.x * 10f;
        bulletClone.transform.localScale = new Vector2(transform.localScale.x, 1f);
    }
}
