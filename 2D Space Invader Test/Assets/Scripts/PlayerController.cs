using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public int lvl { get; private set; }
    [field: SerializeField] public int exp { get; private set; }
    [field: SerializeField] public CanvasManager canvasManager { get; private set; }
    [field: SerializeField] public Rigidbody2D playerControllerRb { get; private set; }
    [field: SerializeField] public WallBounce playerWallBounce { get; private set; }
    [field: SerializeField] public Projectile projectilePrefab { get; private set; }
    [field: SerializeField] public float firingRate { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    [field: SerializeField] public bool canShoot = true;

    [field: SerializeField] public GameObject sideGunner { get; private set; }
    [field: SerializeField] public SideGunner[] sideGunnerChild { get; private set; }
    [field: SerializeField] public GameObject laserHead { get; private set; }
    [field: SerializeField] public GameObject freezeGun { get; private set; }
    [field: SerializeField] public GameObject hitEffectPrefab { get; private set; }
    [field: SerializeField] public ScreenShake screenShake { get; private set; }

    private void Awake() {
        hp = 3;
        lvl = 1;
        exp = 0;
        playerWallBounce = GetComponent<WallBounce>();
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            screenShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShake>();
            canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
            canvasManager.playerHP.text = hp.ToString();
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        if (!canShoot) { return; }
        Shoot();
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     GainRandomPowerUp();
        // }
    }

    private void Movement() {
        float horizontalInput = Input.GetAxis("Horizontal");
    	float verticalInput = Input.GetAxis("Vertical");

    	Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

    	if(!playerWallBounce.isBouncing) playerControllerRb.velocity = movement * moveSpeed;
    }

    private void Shoot() {
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + firingRate;
            Instantiate(this.projectilePrefab, this.transform.position, Quaternion.Euler(0, 0, 90));
        }
    }

    public void TakeDamage() {
        if (!canShoot) { return; }
        AudioManager.instance.Play("Damaged");
        canvasManager.ShowDialogue("OUCH!");
        StartCoroutine(TimeSlowWhenDamaged());
        screenShake.TriggerScreenShake();
        Instantiate(this.hitEffectPrefab, this.transform.position, Quaternion.identity);
        hp -= 1;
        canvasManager.playerHP.text = hp.ToString();
        if (hp <= 0) {
            Time.timeScale = 0.1f;
            canvasManager.fadeoutPanel.SetActive(true);
            canvasManager.fadeoutPanel.GetComponent<Animator>().Play("Fade_Out");
            Invoke("LoadMainMenuScene", 0.7f);

            AudioManager.instance.Play("Dead");
            gameObject.SetActive(false);
        }
    }

    public void GainExp(int value) {
        exp += value;
        if (exp >= lvl * 15) {
            lvl += 1;
            GainRandomPowerUp();
        }
    }
    private void GainRandomPowerUp() {
        // 1 = sidegunner, 2 = laserhead, 3 = Increased Firing Rate, 4 = HP+1, 5 = Freeze Gun
        int min = 1, max = 5;
        int rand = Random.Range(1, 6);
        int rand2 = Random.Range(1, 6);
        if (rand == rand2)
            rand = min + (rand - min + Random.Range(1, max - min)) % (max - min);
        
        canvasManager.ShowWeaponSelectionUI(rand, rand2);
        Time.timeScale = 0;
    }

    public void GainPowerUp(int selectedPowerUp) {
        // 1 = sidegunner, 2 = laserhead, 3 = Increased Firing Rate, 4 = HP+1, 5 = Freeze Gun

        switch(selectedPowerUp) {
            case 1:
                foreach(SideGunner sideGunners in sideGunnerChild) {
                    sideGunners.gameObject.SetActive(true);
                }
                break;
            case 2:
                laserHead.SetActive(true);
                break;
            case 3:
                firingRate -= 0.1f;
                if (firingRate < 0.3f) firingRate = 0.3f;
                break;
            case 4:
                hp += 1;
                if (hp > 3) {
                    hp = 3;
                    canvasManager.ShowDialogue("YOUR HEALTH IS MAX OUT!");
                }
                canvasManager.playerHP.text = hp.ToString();
                break;
            case 5:
                freezeGun.SetActive(true);
                break;
            default:
                break;
        }

        canvasManager.HideWeaponSelectionUI();
        Time.timeScale = 1;
    }

    public void SetMoveSpeed(float _moveSpeed) {
        moveSpeed = _moveSpeed;
    }

    IEnumerator TimeSlowWhenDamaged() {
        if (!canShoot) { yield break; }
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
    }

    public void LoadMainMenuScene () {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameScene () {
        SceneManager.LoadScene("GameScene");
    }
}
