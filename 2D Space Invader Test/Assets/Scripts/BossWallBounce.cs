using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWallBounce : MonoBehaviour
{
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public bool isBouncing { get; private set; }
    [field: SerializeField] public float bounceForce { get; private set; }

    private void Awake() {
        boss = GetComponent<Boss>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.name == "Map" )
        {
            boss.bossRb.AddForce(collision.contacts[0].normal * bounceForce);
            isBouncing = true;
            Invoke("StopBounce", 0.3f);
        }
    }
    void StopBounce()
    {
        isBouncing = false;
    }
}
