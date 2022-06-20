using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [field: SerializeField] public int damageValue { get; set; }
    [field: SerializeField] public Vector3 direction { get; set; }
    [field: SerializeField] public float projectileSpeed { get; private set; }

    private void Update() {
        this.transform.position += direction * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case "Enemy":
                Destroy(this.gameObject);
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageValue, "fromBoss");
                break;
            case "ScreenEdge":
            case "PowerUpEquipment":
            case "Shield":
                Destroy(this.gameObject);
                break;
            case "Player":
                Destroy(this.gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                break;
            default:
                break;
        }
    }
}
