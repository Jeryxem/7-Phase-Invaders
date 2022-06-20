using UnityEngine;
using System.Collections;

public class FreezeGun : MonoBehaviour
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
        if (!playerController.canShoot) { return; }
        StartCoroutine(Shoot());
        UpdateLineRendererPosition();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!playerController.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile-Boss":
            case "Enemy":
            case "Shield":
                AudioManager.instance.Play("Damaged");
                playerController.canvasManager.ShowDialogue("Your Freeze Gun Got Destroyed!");
                boss.SetMoveSpeed(5f);
                TimeSlowWhenDestroyed();
                Instantiate(playerController.hitEffectPrefab, this.transform.position, Quaternion.identity);

                lineRenderer.enabled = false;
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    IEnumerator Shoot() {
        if (!playerController.canShoot) { yield break; }
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + playerController.firingRate + chargeTime;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, boss.transform.position);

            lineRenderer.enabled = true;

            yield return new WaitForSeconds(1.5f);

            lineRenderer.enabled = false;
            Instantiate(freezeEffectPrefab, boss.transform.position, Quaternion.identity);
            AudioManager.instance.Play("FreezeGun");
            boss.SetMoveSpeed(0f);
            StartCoroutine(ResetSpeed(5f));
        }
    }

    private void UpdateLineRendererPosition() {
        if (lineRenderer.enabled == true) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, boss.transform.position);
        }
    }

    private IEnumerator ResetSpeed(float moveSpeed) {
        yield return new WaitForSeconds(2f);
        boss.SetMoveSpeed(moveSpeed);
    }

    private void TimeSlowWhenDestroyed() {
        Time.timeScale = 0.1f;
        Invoke("TimeBackToNormal", 0.1f);
    }

    private void TimeBackToNormal() {
        if (playerController.canvasManager.weaponSelectionUI.activeSelf) { return; }
        Time.timeScale = 1;
    }
}
