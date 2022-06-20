using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public BossLaserHead bossLaserHead { get; private set; }
    [field: SerializeField] public GameObject impactEffectPrefab { get; private set; }
    [field: SerializeField] public int damageValue { get; private set; }

    private void Awake() {
        bossLaserHead = GetComponentInParent<BossLaserHead>();
        boss = bossLaserHead.boss;
        impactEffectPrefab = bossLaserHead.impactEffectPrefab;
        damageValue = bossLaserHead.damageValue;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case "Enemy":
                // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageValue, "fromBoss");
                break;
            case "Player":
                // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                break;
            default:
                break;
        }
    }
}
