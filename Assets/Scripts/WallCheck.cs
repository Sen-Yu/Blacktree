using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public Monster owner;

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Platform"))
        {
            owner.MonsterFlip( );
        }
    }
}
