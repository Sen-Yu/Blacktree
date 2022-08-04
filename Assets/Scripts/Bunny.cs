using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Monster
{
    public enum State
    {
        Idle,
        Run,
        Attack,
        Jump
    };
    public State currentState = State.Idle;

    public Transform[] wallCheck;
    WaitForSeconds Delay500 = new WaitForSeconds(0.5f);

    Vector2 boxColliderOffset;
    Vector2 boxColliderJumpOffset;


    void Awake()
    {
        base.Awake();
        moveSpeed = 3f;
        jumpPower = 20f;
        currentHp = 6;
        atkCooltime = 3f;
        atkCooltimeCalc = atkCooltime;

        boxColliderOffset = boxCollider.offset;
        boxColliderJumpOffset = new Vector2(boxColliderOffset.x, 1f);

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
        boxCollider.offset = boxColliderOffset;
        yield return Delay500;
        currentState = State.Run;
    }

    IEnumerator Run()
    {
        yield return null;
        float runTime = Random.Range(2f, 4f);
        
        while (runTime >= 0f)
        {
            runTime -= Time.deltaTime;
            if(!isHit)
            {
                MyAnimSetTrigger("Run");
                rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);

                if (!Physics2D.OverlapCircle(wallCheck[0].position, 0.05f, layerMask) &&
                    Physics2D.OverlapCircle(wallCheck[1].position, 0.05f, layerMask))
                {
                    if(Random.Range(-1, 2) ==0){
                        MonsterFlip();
                    }
                    else{
                        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                    }
                }

                if(IsPlayerDir() && isGround && canAtk)
                {
                    if(Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < 5f)
                    {
                        currentState = State.Attack;
                        break;
                    }
                }
            }
            yield return null;
        }
        /*if (currentState != State.Attack && currentState != State.Jump)
             {
                if (!IsPlayerDir())
                {
                    MonsterFlip();
                }
             }*/
    }

    IEnumerator Attack ()
    {
        yield return null;
        
        if(!isHit && isGround)
        {
            boxCollider.offset = boxColliderJumpOffset;
            canAtk = false;
            rb.velocity = new Vector2(-transform.localScale.x * 6f, jumpPower);
            MyAnimSetTrigger("Attack");
            yield return Delay500;
            currentState = State.Idle;
        }
    }

    IEnumerator Jump()
    {
        yield return null;
        boxCollider.offset = boxColliderJumpOffset;

        rb.velocity = new Vector2 (-transform.localScale.x * 6f, jumpPower);
        MyAnimSetTrigger("Attack");
        yield return Delay500;
        currentState = State.Idle;
    }

    void Update() 
    {
        {
            GroundCheck();
            if(!isHit && isGround && !IsPlayingAnim("Run"))
            {
                boxCollider.offset = boxColliderOffset;
                MyAnimSetTrigger("Idle");
            }
        }
    }
}
