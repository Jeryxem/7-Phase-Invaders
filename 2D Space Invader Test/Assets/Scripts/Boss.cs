using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public int lvl { get; private set; }
    [field: SerializeField] public int exp { get; private set; }
    [field: SerializeField] public CanvasManager canvasManager { get; private set; }
    [field: SerializeField] public Rigidbody2D bossRb { get; private set; }
    [field: SerializeField] public BossWallBounce bossWallBounce { get; private set; }
    [field: SerializeField] public BossProjectile bossProjectilePrefab { get; private set; }
    [field: SerializeField] public float firingRate { get; private set; }
    [field: SerializeField] public float nextFiringTime { get; private set; }
    private Vector3 targetPoint;
    [field: SerializeField] public bool canShoot = true;

    [field: SerializeField] public GameObject sideGunner { get; private set; }
    [field: SerializeField] public BossSideGunner[] bossSideGunnerChild { get; private set; }
    [field: SerializeField] public GameObject laserHead { get; private set; }
    [field: SerializeField] public GameObject freezeGun { get; private set; }
    [field: SerializeField] public GameObject hitEffectPrefab { get; private set; }
    [field: SerializeField] public ScreenShake screenShake { get; private set; }
    [field: SerializeField] public BossEnemySpawner bossEnemySpawner { get; private set; }
    [field: SerializeField] public EnemySpawner enemySpawner { get; private set; }
    [field: SerializeField] public bool testing { get; private set; }
    

    private void Awake() {
        hp = 3;
        lvl = 1;
        exp = 0;
        bossWallBounce = GetComponent<BossWallBounce>();

        if (SceneManager.GetActiveScene().name != "MainMenu") {
            bossEnemySpawner = GameObject.Find("EnemySpawner").GetComponent<BossEnemySpawner>();
            enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
            screenShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShake>();
            canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
            canvasManager.bossHP.text = hp.ToString();
        }
        
        targetPoint = new Vector3(Random.Range(16,25), Random.Range(-9,10), 0);

        if (testing) {
            hp = 9999;
            canvasManager.bossHP.text = hp.ToString();
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     TakeDamage();
        // }
        if (!canShoot) { return; }
        Shoot();
    }

    private void Movement() {
        if((bossRb.transform.position - targetPoint).magnitude < 3) {
            targetPoint = new Vector3(Random.Range(16,25), Random.Range(-9,10), 0);
        }
        transform.position = Vector2.MoveTowards(bossRb.transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }

    private void Shoot() {
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + firingRate;
            Instantiate(this.bossProjectilePrefab, this.transform.position, Quaternion.Euler(0, 0, 90));
        }
    }

    public void TakeDamage() {
        if (!canShoot) { return; }
        AudioManager.instance.Play("Damaged");
        canvasManager.ShowDialogue("NICE!");
        StartCoroutine(TimeSlowWhenDamaged());
        screenShake.TriggerScreenShake();
        Instantiate(this.hitEffectPrefab, this.transform.position, Quaternion.identity);
        hp -= 1;
        canvasManager.bossHP.text = hp.ToString();
        if (hp <= 0) {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies) {
                enemy.Death();
            }
            AudioManager.instance.Play("AllDead");
            bossEnemySpawner.StopSpawning();
            enemySpawner.StopSpawning();

            canShoot = false;
            moveSpeed = 0;
            canvasManager.playerController.canShoot = false;
            canvasManager.playerController.moveSpeed = 0f;

            if (enemySpawner.currentLevel >= 7) { 
                Time.timeScale = 0.1f;
                canvasManager.fadeoutPanel.SetActive(true);
                canvasManager.fadeoutPanel.GetComponent<Animator>().Play("Fade_Out");

                AudioManager.instance.Play("Dead");
                Invoke("LoadCreditScene", 0.7f);
            }

            Invoke("PreparingNextLevel", 2f);
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
        int rand = Random.Range(1,6);
        switch(rand) {
            case 1:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                canvasManager.ShowDialogue("BEWARE ENEMY SIDE GUNNER!");
                break;
            case 2:
                laserHead.SetActive(true);
                canvasManager.ShowDialogue("BEWARE ENEMY LASER HEAD!");
                break;
            case 3:
                firingRate -= 0.1f;
                if (firingRate < 0.3f) firingRate = 0.3f;
                canvasManager.ShowDialogue("BEWARE ENEMY ADDED FIRE RATE!");
                break;
            case 4:
                hp += 1;
                // if (hp > 3) hp = 3;
                canvasManager.ShowDialogue("BEWARE ENEMY HEALTH RESTORED!");
                canvasManager.bossHP.text = hp.ToString();
                break;
            case 5:
                freezeGun.SetActive(true);
                canvasManager.ShowDialogue("BEWARE ENEMY FREEZE GUN!");
                break;
            default:
                break;
        }
    }

    public void SetMoveSpeed(float _moveSpeed) {
        moveSpeed = _moveSpeed;
    }

    IEnumerator TimeSlowWhenDamaged() {
        if (!canShoot || enemySpawner.currentLevel >= 7) { yield break; }
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
    }

    public void PreparingNextLevel() {
        switch((enemySpawner.currentLevel + 1)) {
            case 2:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                canvasManager.ShowDialogue("GET READY FOR PHASE " + (enemySpawner.currentLevel + 1) + " !");
                break;
            case 3:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                laserHead.SetActive(true);
                canvasManager.ShowDialogue("GET READY FOR PHASE " + (enemySpawner.currentLevel + 1) + " !");
                break;
            case 4:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                laserHead.SetActive(true);
                Invoke("WarningMsg1", 2.1f);
                canvasManager.ShowDialogue("GET READY FOR PHASE " + (enemySpawner.currentLevel + 1) + " !");
                break;
            case 5:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                laserHead.SetActive(true);
                freezeGun.SetActive(true);
                canvasManager.ShowDialogue("GET READY FOR PHASE " + (enemySpawner.currentLevel + 1) + " !");
                break;
            case 6:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                laserHead.SetActive(true);
                freezeGun.SetActive(true);
                canvasManager.ShowDialogue("GET READY FOR PHASE " + (enemySpawner.currentLevel + 1) + " !");
                Invoke("WarningMsg2", 2.1f);
                break;
            case 7:
                foreach(BossSideGunner bossSideGunners in bossSideGunnerChild) {
                    bossSideGunners.gameObject.SetActive(true);
                }
                laserHead.SetActive(true);
                freezeGun.SetActive(true);
                firingRate = 0.3f;
                canvasManager.ShowDialogue("GET READY FOR THE FINAL PHASE!");
                Invoke("WarningMsg3", 2.1f);
                break;
            default:
                break;
        }

        canvasManager.phase.text = "PHASE:" + (enemySpawner.currentLevel + 1);

        hp = bossEnemySpawner.currentLevel + 2;
        canvasManager.bossHP.text = hp.ToString();

        moveSpeed = 5f;
        canvasManager.playerController.moveSpeed = 5f;

        Invoke("BeginNextLevel", 4.5f);
    }

    public void BeginNextLevel() {
        canvasManager.ShowDialogue("BEGIN!");

        canShoot = true;
        canvasManager.playerController.canShoot = true;

        bossEnemySpawner.NextLevel();
        enemySpawner.NextLevel();
    }

    public void WarningMsg1 () {
        canvasManager.ShowDialogue("INDESTRUCTIBLE SIDE GUNNER!");
    }
    public void WarningMsg2 () {
        canvasManager.ShowDialogue("INDESTRUCTIBLE FREEZE GUN!");
    }
    public void WarningMsg3 () {
        canvasManager.ShowDialogue("EXTREME FIRE RATE!");
    }
    public void LoadCreditScene () {
        Time.timeScale = 1;
        SceneManager.LoadScene("Credit");
    }
}
