using System.Collections;
using UnityEngine;

public class BossFreezeGun : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public GameObject freezeEffectPrefab { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public float chargeTime { get; private set; }
    [field: SerializeField] public LineRenderer lineRenderer { get; private set; }

    private void Awake() {
        playerController = GameObject.Find("PlayerTest").GetComponent<PlayerController>();
        boss = GameObject.Find("EnemyBossTest").GetComponent<Boss>();
    }

    private void Update() {
        if (!boss.canShoot) { return; }
        StartCoroutine(Shoot());
        UpdateLineRendererPosition();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!boss.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile":
            case "Enemy":
            case "Shield":
                if ((boss.enemySpawner.currentLevel + 1) > 6) return;
                AudioManager.instance.Play("Damaged");
                boss.canvasManager.ShowDialogue("Enemy Freeze Gun Got Destroyed!");
                playerController.SetMoveSpeed(5f);
                TimeSlowWhenDestroyed();
                Instantiate(boss.hitEffectPrefab, this.transform.position, Quaternion.identity);

                lineRenderer.enabled = false;
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

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, playerController.transform.position);

            lineRenderer.enabled = true;

            yield return new WaitForSeconds(1.5f);

            lineRenderer.enabled = false;
            Instantiate(freezeEffectPrefab, playerController.transform.position, Quaternion.identity);
            AudioManager.instance.Play("FreezeGun");
            playerController.SetMoveSpeed(0f);
            StartCoroutine(ResetSpeed(5f));
        }
    }

    private void UpdateLineRendererPosition() {
        if (lineRenderer.enabled == true) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, playerController.transform.position);
        }
    }

    private IEnumerator ResetSpeed(float moveSpeed) {
        yield return new WaitForSeconds(2f);
        playerController.SetMoveSpeed(moveSpeed);
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
