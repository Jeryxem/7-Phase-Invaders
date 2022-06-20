using System.Collections;
using UnityEngine;

public class LaserHead : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public GameObject impactEffectPrefab { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public float chargeTime { get; private set; }
    [field: SerializeField] public int damageValue { get; private set; }
    [field: SerializeField] public LineRenderer lineRenderer { get; private set; }
    [field: SerializeField] public GameObject laserCollider { get; private set; }
    [field: SerializeField] public GameObject laserWarning { get; private set; }
    private bool warningAlert = false;

    private void Awake() {
        playerController = GameObject.Find("PlayerTest").GetComponent<PlayerController>();
    }

    void Update()
    {
        StartCoroutine(WarningAlert());

        UpdateLaserPosition();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!playerController.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile-Boss":
            case "Enemy":
            case "Shield":
                warningAlert = false;
                AudioManager.instance.Play("Damaged");
                playerController.canvasManager.ShowDialogue("Your Laser Head Got Destroyed!");
                TimeSlowWhenDestroyed();
                Instantiate(playerController.hitEffectPrefab, this.transform.position, Quaternion.identity);

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
        if (!playerController.canShoot) { yield break; }
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + playerController.firingRate + chargeTime;
            // Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.identity);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.right * 50);

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
            lineRenderer.SetPosition(1, transform.position + transform.right * 50);
        }
    }

    IEnumerator WarningAlert() {
        if (!playerController.canShoot) { yield break; }
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
        if (playerController.canvasManager.weaponSelectionUI.activeSelf) { return; }
        Time.timeScale = 1;
    }
}
