using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D Capsulecollider;

    public GameManager gameManager;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Capsulecollider = GetComponent<CapsuleCollider2D>();

        Think();
    }

    void Update()
    {
        /*if (Input.GetKey("x"))
        {
            if(Mathf.Abs(GameObject.FindWithTag("Player").transform.position.x - transform.position.x) < 1 && Mathf.Abs(GameObject.FindWithTag("Player").transform.position.y - transform.position.y) < 1)
            {
                DeActive();
            }
        }*/
    }

    void FixedUpdate()
    {
        if(gameObject.tag != "EnemyDead")
        {
            //Move
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

            //Falling Check
            Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider == null)
            {
                Turn();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.transform.CompareTag("PlayerWeapon") && gameObject.transform.CompareTag("Enemy"))
            {
                gameObject.tag= "EnemyDead";
                OnDamaged(GameObject.FindWithTag("Player").transform.position);
            }
    }


    //Random Move
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove > 0;

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        if(gameObject.tag != "EnemyDead")
        {
            Invoke("Think", nextThinkTime);
        }
    }

    //Turn Back
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove > 0;
        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged(Vector2 targetPos)
    {
        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 4, ForceMode2D.Impulse);
        OnDie();
        
        /*//Color Change
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip
        spriteRenderer.flipY = true;

        //Reaction Force
        rigid.AddForce(Vector2.up * 1, ForceMode2D.Impulse);*/

    }

   public void OnDie()
    {
        //Sprite Flip
        spriteRenderer.flipY = true;
        anim.SetBool("isDead", true);
        //PlaySound("DIE");
        
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    


}
