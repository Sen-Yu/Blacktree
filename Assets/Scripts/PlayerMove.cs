using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[SelectionBase]

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;

    public GameObject weaponCollider;
    public GameObject consumCollider;
    SpriteRenderer weaponSpriteRenderer;
    Vector2 weaponVector;
    Animator weaponAnim;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    AudioSource audioSource;

    public bool facingRight = true;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        weaponAnim = weaponCollider.GetComponent<Animator>();
        //weaponSpriteRenderer = weaponCollider.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.000001f, rigid.velocity.y);

        //Set Sprite Direction
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Idle, Run Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isRunning", false);
        else
            anim.SetBool("isRunning", true);


        /*if (Input.GetButton("Horizontal"))
        {
            //weaponSpriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;\
            if(spriteRenderer.flipX)
            {
                weaponVector = weaponCollider.transform.localPosition;
                weaponVector.x = -0.9f;
                weaponCollider.transform.localPosition = weaponVector;
            }
            else
            {
                weaponVector = weaponCollider.transform.localPosition;
                weaponVector.x = 0.9f;
                weaponCollider.transform.localPosition = weaponVector;
            }
        }*/

        if (Input.GetKey("z"))
        {
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
                anim.SetTrigger("doAttack");
                weaponAnim.SetTrigger("doAttack");
            }
        }
    }

    void FixedUpdate()
    {
        //Move By Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);


        //Landing Platform
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rigid.velocity.y < 0 && rayHit.collider != null && rayHit.distance < 0.5f)
        {
            anim.SetBool("isJumping", false);
        }

        float d = Input.GetAxis("Horizontal");
        if(d > 0 && !facingRight)
            Flip();
        else if(d < 0 && facingRight)
            Flip();
    }

    //Collision Event
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.genePoint += 50;
            else if (isSilver)
                gameManager.genePoint += 100;
            else if (isGold)
                gameManager.genePoint += 200;

            //Deactive Item
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }

        if (collision.gameObject.tag == "Portal")
        {
            gameManager.NextStage();
            PlaySound("FINISH");
        }

    }


    /*void OnAttack(Transform enemy)
    {
        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");
    }*/

    void OnDamaged(Vector2 targetPos)
    {
        //Health Down
        gameManager.HealthDown();

        //Change Layer
        gameObject.layer = 10;

        //Color Change
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 12, ForceMode2D.Impulse);

        PlaySound("DAMAGED");

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 9;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Color Change
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip
        spriteRenderer.flipY = true;

        //Reaction Force
        rigid.AddForce(Vector2.up * 1, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }


    void Flip ()
    {
        facingRight = !facingRight;
        Vector3 theScale = weaponCollider.transform.localPosition;
        theScale.x *= -1;
        weaponCollider.transform.localPosition = theScale;
        weaponCollider.transform.Rotate(0,180,0);
    }

    /*public void WeaponColliderOnOff()
    {
        weaponCollider.SetActive (!weaponCollider.activeInHierarchy);
    }*/
}
