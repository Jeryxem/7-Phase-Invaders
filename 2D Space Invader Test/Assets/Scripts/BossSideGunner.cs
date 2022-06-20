using UnityEngine;

public class BossSideGunner : MonoBehaviour
{
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public BossProjectile bossProjectilePrefab { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public float chargeTime { get; private set; }

    private void Awake() {
        boss = GameObject.Find("EnemyBossTest").GetComponent<Boss>();
    }

    void Update()
    {
        if (!boss.canShoot) { return; }
        Shoot();
    }

    private void Shoot() {
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + boss.firingRate + chargeTime;
            Instantiate(this.bossProjectilePrefab, this.transform.position, Quaternion.Euler(0, 0, 90));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!boss.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile":
            case "Enemy":
            case "Shield":
                if ((boss.enemySpawner.currentLevel + 1) > 4) return;
                AudioManager.instance.Play("Damaged");
                boss.canvasManager.ShowDialogue("Enemy SideGunner Got Destroyed!");
                TimeSlowWhenDestroyed();
                Instantiate(boss.hitEffectPrefab, this.transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void TimeSlowWhenDestroyed() {
        Time.timeScale = 0.1f;
        Invoke("TimeBackToNormal", 0.1f);
    }

    private void TimeBackToNormal() {
        if (boss.canvasManager.weaponSelectionUI.activeSelf) { return; }
        Time.timeScale = 1;
    }
}
