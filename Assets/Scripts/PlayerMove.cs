using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    //public GameObject weaponCollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    AudioSource audioSource;

    public float jumpHeight = 3f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Jump
        if (Input.GetButton("Jump") && !anim.GetBool("isJumping"))
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpHeight * - Physics.gravity.y);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
            // rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.000001f, rigid.velocity.y);

        //Set Sprite Direction
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Idle, Run Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isRunning", false);
        else { 
            anim.SetBool("isRunning", true);
           
        }

        //Spin - Attack
        if(Input.GetKey(KeyCode.Z))
        {
            anim.SetBool("isSpinning", true);
            KeyDown_Z();
            //add attack state -> not do damaged
        }
        else { 
            anim.SetBool("isSpinning", false);
           
        }


    }
    private void KeyDown_Z()
    {
        Debug.Log("Z");
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
        if (rigid.velocity.y < 0 && rayHit.collider != null && rayHit.distance < 1.1f)
        {
            anim.SetBool("isJumping", false);
        }
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


    void OnAttack(Transform enemy)
    {
        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");
    }

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

    /*public void WeaponColliderOnOff ()
    {
        weaponCollider.SetActive (weaponCollider.activeHierarchy);
    }*/
}
