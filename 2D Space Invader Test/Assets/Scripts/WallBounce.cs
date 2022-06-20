using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public bool isBouncing { get; private set; }
    [field: SerializeField] public float bounceForce { get; private set; }

    private void Awake() {
        playerController = GetComponent<PlayerController>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.name == "Map" )
        {
            AudioManager.instance.Play("Hit");
            Instantiate(playerController.hitEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            playerController.playerControllerRb.AddForce(collision.contacts[0].normal * bounceForce);
            isBouncing = true;
            Invoke("StopBounce", 0.3f);
        }
    }
    void StopBounce()
    {
        isBouncing = false;
    }
}
