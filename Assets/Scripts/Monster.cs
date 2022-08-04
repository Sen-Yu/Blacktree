using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int currentHp = 1;
    public float moveSpeed = 5f;
    public float jumpPower = 10;
    public float atkCooltime = 3f;
    public float atkCooltimeCalc = 3f;

    public bool isHit = false;
    public bool isGround = true;
    public bool canAtk = true;
    public bool MonsterDirRight;

    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    public GameObject hitBoxCollider;
    public Animator Anim;
    public LayerMask layerMask;
    

    protected void Awake()
    {  
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();

        StartCoroutine(CalcCoolTime());
        StartCoroutine(ResetCollider());
    }

    // Reset hitbox
    IEnumerator ResetCollider()
    {
        while (true)
        {
            yield return null;
            if (!hitBoxCollider.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }

    // Set cooltime
    IEnumerator CalcCoolTime()
    {
        while (true)
        {
            yield return null;
            if (!canAtk)
            {
                atkCooltimeCalc -=Time.deltaTime;
                if (atkCooltimeCalc <= 0)
                {
                    atkCooltimeCalc = atkCooltime;
                    canAtk = true;
                }
            }
        }
    }

    // Check animation is playing
    public bool IsPlayingAnim(string animName)
    {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            return true;
        }
        return false;
    }

    public void MyAnimSetTrigger(string animName)
    {
        if(!IsPlayingAnim(animName))
        {
            Anim.SetTrigger(animName);
        }
    }

    public void MonsterFlip()
    {
        MonsterDirRight = !MonsterDirRight;
        Vector3 thisScale = transform.localScale;

        if (MonsterDirRight)
        {
            thisScale.x = -Mathf.Abs(thisScale.x);
        }
        else
        {
            thisScale.x = Mathf.Abs(thisScale.x);
        }

        transform.localScale = thisScale;
        rb.velocity = Vector2.zero;
    }

    protected bool IsPlayerDir()
    {
        if (transform.position.x < GameObject.FindWithTag("Player").transform.position.x ? MonsterDirRight : !MonsterDirRight)
        {
            return true;
        }
        return false;
    }

    protected void GroundCheck(){
        if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.size, 0, Vector2.down, 0.05f, layerMask))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public void TakeDamage(int dam)
    {
        currentHp -= dam;
        isHit = true;
        hitBoxCollider.SetActive(false);
    }

}
