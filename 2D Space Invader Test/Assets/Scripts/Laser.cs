using UnityEngine;

public class Laser : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public LaserHead laserHead { get; private set; }
    [field: SerializeField] public GameObject impactEffectPrefab { get; private set; }
    [field: SerializeField] public int damageValue { get; private set; }

    private void Awake() {
        laserHead = GetComponentInParent<LaserHead>();
        playerController = laserHead.playerController;
        impactEffectPrefab = laserHead.impactEffectPrefab;
        damageValue = laserHead.damageValue;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case "Enemy":
                // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageValue, "fromPlayer");
                break;
            case "Boss":
                // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
                other.gameObject.GetComponent<Boss>().TakeDamage();
                break;
            default:
                break;
        }
    }
}
