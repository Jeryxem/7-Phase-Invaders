using UnityEngine;

public class Shield : MonoBehaviour
{
    [field: SerializeField] public int hp { get; private set; }

    private void Awake() {
        hp = 2;
    }    

    private void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case "Projectile-Boss":
            case "Projectile":
                AudioManager.instance.Play("Hit");
                hp -= 1;
                if (hp <= 0) Destroy(gameObject);
                break;
            case "Player":
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                break;
            case "Boss":
                Destroy(gameObject);
                other.gameObject.GetComponent<Boss>().TakeDamage();
                break;
            default:
                break;
        }
    }
}
