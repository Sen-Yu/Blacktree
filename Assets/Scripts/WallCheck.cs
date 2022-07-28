using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public Monster owner;

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);

        if(collision.transform.CompareTag("Platform"))
        {
            Debug.Log("fliped");
            owner.MonsterFlip( );
        }
    }
}
