using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public Boss boss { get; private set; }
    [field: SerializeField] public EnemySpawner enemySpawner { get; private set; }
    [field: SerializeField] public BossEnemySpawner bossEnemySpawner { get; private set; }
    [field: SerializeField] public EnemyState enemyState { get; private set; }

    [field: SerializeField] public int spawnCost { get; private set; }
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public float movementSpeed { get; private set; }
    [field: SerializeField] public int exp { get; private set; }
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    [field: SerializeField] public Vector3 target { get; set; }
    [field: SerializeField] public Vector3 targetBossSide { get; set; }
    [field: SerializeField] public float timeToTargetPosition { get; private set; }
    [field: SerializeField] public GameObject shield { get; set; }
    [field: SerializeField] public GameObject deathEffect { get; set; }
    [field: SerializeField] public Animator animator { get; set; }

    private void Awake() {
        enemyState = EnemyState.SettingUp;
        SetData();
        playerController = GameObject.Find("PlayerTest").GetComponent<PlayerController>();
        boss = GameObject.Find("EnemyBossTest").GetComponent<Boss>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        bossEnemySpawner = GameObject.Find("EnemySpawner").GetComponent<BossEnemySpawner>();
        target = enemySpawner.targetPosition;
        targetBossSide = bossEnemySpawner.targetPosition;
        shield = gameObject.transform.Find("Shield").gameObject;
        animator = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate() {
        switch(enemyState) {
            case EnemyState.SettingUp:
                MoveToPosition();
                break;
            case EnemyState.Moving:
                Movement();
                break;
            case EnemyState.ChasePlayer:
                ChasePlayer();
                break;
            case EnemyState.ChaseBoss:
                ChaseBoss();
                break;
            default:
                MoveToPosition();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 9) { // PowerUpEquipment Layer Number
            Death();
        }
        switch(other.gameObject.tag) {
            case "SuicideTrigger-Left":
                animator.Play("ChasePlayer");
                break;
            case "SuicideTrigger-Right":
                animator.Play("ChaseBoss");
                break;
            case "Player":
                Death();
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                break;
            case "Boss":
                Death();
                other.gameObject.GetComponent<Boss>().TakeDamage();
                break;
            default:
                break;
        }
    }

    public void SetData() {
        spawnCost = enemyData.spawnCost;
        hp = enemyData.hp;
        movementSpeed = enemyData.movementSpeed;
        exp = enemyData.exp;
    }

    public void SetEnemyState(EnemyState enemyState) {
        this.enemyState = enemyState;
    }

    public virtual void RemoveShield() {
        shield.SetActive(false);
    }

    protected virtual void MoveToPosition() {
        Vector3 spawnPosition = transform.position;
        Vector3 targetPosition = target;
        transform.position = Vector3.Lerp(spawnPosition, targetPosition, timeToTargetPosition);
    }

    protected virtual void Movement() {
        Vector3 pos1 = target + new Vector3(transform.position.x - target.x - 0.01f, 1f, 0);
        Vector3 pos2 = target + new Vector3(transform.position.x - target.x - 0.01f, -1f, 0);

        transform.position = Vector3.Lerp (pos1, pos2, Mathf.PingPong(Time.time * movementSpeed, 1.0f));
    }

    public void ChasePlayer() {
        transform.position = Vector2.MoveTowards(transform.position, playerController.transform.position, movementSpeed * Time.deltaTime);
    }
    public void ChaseBoss() {
        transform.position = Vector2.MoveTowards(transform.position, boss.transform.position, movementSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage, string damageDealer) {
        if (enemyState == EnemyState.SettingUp) { 
            AudioManager.instance.Play("Hit");
            return; 
        }
        hp -= damage;
        AudioManager.instance.Play("Hit");
        if (hp <= 0) {
            Death();
            if (damageDealer == "fromPlayer") {
                playerController.GainExp(exp);
            } else {
                boss.GainExp(exp);
            }
        }
    }

    public void Death() {
        AudioManager.instance.Play("Dead");
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

public enum EnemyState {
    SettingUp,
    Moving,
    ChasePlayer,
    ChaseBoss
}
