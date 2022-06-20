using System.Collections;
using UnityEngine;

public class BossLaserHead : MonoBehaviour
{
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public GameObject impactEffectPrefab { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public float chargeTime { get; private set; }
    [field: SerializeField] public int damageValue { get; private set; }
    [field: SerializeField] public LineRenderer lineRenderer { get; private set; }
    [field: SerializeField] public GameObject laserCollider { get; private set; }
    [field: SerializeField] public GameObject laserWarning { get; private set; }
    private bool warningAlert = false;

    private void Awake() {
        boss = GameObject.Find("EnemyBossTest").GetComponent<Boss>();
    }

    void Update()
    {
        StartCoroutine(WarningAlert());

        UpdateLaserPosition();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!boss.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile":
            case "Enemy":
            case "Shield":
                warningAlert = false;
                AudioManager.instance.Play("Damaged");
                boss.canvasManager.ShowDialogue("Enemy Laser Head Got Destroyed!");
                TimeSlowWhenDestroyed();
                Instantiate(boss.hitEffectPrefab, this.transform.position, Quaternion.identity);

                lineRenderer.enabled = false;
                laserCollider.SetActive(false);
                warningAlert = false;
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    IEnumerator Shoot() {
        if (!boss.canShoot) { yield break; }
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + boss.firingRate + chargeTime;
            // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + -transform.right * 50);

            lineRenderer.enabled = true;
            laserCollider.SetActive(true);
            AudioManager.instance.Play("Laser");

            yield return new WaitForSeconds(0.5f);

            lineRenderer.enabled = false;
            laserCollider.SetActive(false);
            warningAlert = false;
        }
    }

    private void UpdateLaserPosition() {
        if (lineRenderer.enabled == true) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + -transform.right * 50);
        }
    }

    IEnumerator WarningAlert() {
        if (!boss.canShoot) { yield break; }
        if (Time.time > nextFiringTime && warningAlert == false) {
            warningAlert = true;
            laserWarning.SetActive(true);
            yield return new WaitForSeconds(1f);
            laserWarning.SetActive(false);

            StartCoroutine(Shoot());
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
