using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Monster
{
    public Transform[] wallCheck;

    // Start is called before the first frame update
    private void awake()
    {
        base.Awake();
        moveSpeed = 2f;
        jumpPower = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);
            if (!Physics2D.OverlapCircle(wallCheck[0].position, 0.05f, layerMask) &&
                Physics2D.OverlapCircle(wallCheck[1].position, 0.05f, layerMask) /*&&
                !Physics2D.Raycast ( transform.position, -transform.localScale.x * transform.right, 0.1f, layerMask)*/)
            {
                if(Random.Range(-1, 2) ==0){
                     MonsterFlip();
                }
                else{
                    rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                }
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
