using UnityEngine;

public class SideGunner : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public Projectile projectilePrefab { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public float chargeTime { get; private set; }

    private void Awake() {
        playerController = GameObject.Find("PlayerTest").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!playerController.canShoot) { return; }
        Shoot();
    }

    private void Shoot() {
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + playerController.firingRate + chargeTime;
            Instantiate(this.projectilePrefab, this.transform.position, Quaternion.Euler(0, 0, 90));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!playerController.canShoot) { return; }
        switch(other.gameObject.tag) {
            case "Projectile-Boss":
            case "Enemy":
            case "Shield":
                AudioManager.instance.Play("Damaged");
                playerController.canvasManager.ShowDialogue("Your SideGunner Got Destroyed!");
                TimeSlowWhenDestroyed();
                Instantiate(playerController.hitEffectPrefab, this.transform.position, Quaternion.identity);
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
        if (playerController.canvasManager.weaponSelectionUI.activeSelf) { return; }
        Time.timeScale = 1;
    }
}
