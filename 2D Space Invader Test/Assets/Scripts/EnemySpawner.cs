using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public List<Enemy> enemyPrefabList = new List<Enemy>();
    [field: SerializeField] public Enemy enemyPrefab { get; private set; }
    [field: SerializeField] public Boss boss { get; private set; }

    [field: SerializeField] public int currentLevel { get; private set; }
    [field: SerializeField] public int currentWave { get; private set; }
    [field: SerializeField] public int waveLimit { get; private set; }
    [field: SerializeField] public int spawnLimitCost { get; private set; }
    [field: SerializeField] public int maxEnemyLimit { get; private set; }
    [field: SerializeField] public int enemyInScene { get; private set; }
    [field: SerializeField] public List<Enemy> enemiesList = new List<Enemy>();
    [field: SerializeField] public int[,] waveDataPerLevelArray = new int[,] {
        {0,2},
        {0,3},
        {0,4},
        {2,4},
        {0,4},
        {0,4},
        {2,4},
    };
    [field: SerializeField] public int sharedRandomRange { get; private set; }

    private Vector3 maxTopSpawnPoint;
    private Vector3 maxBottomSpawnPoint;
    private Vector3 unadjustedStartPosition;
    private Vector3 unadjustedEndPosition;
    private Vector3 newUnadjustedStartPosition;
    [field: SerializeField] public Vector3 targetPosition { get; private set; }
    private float lerpValue;
    private float distance;
    private int segmentsToCreate;
    private bool isFirstSegment = true;

    private void Awake() {
        maxTopSpawnPoint = GameObject.Find("PointMaxTop-L").transform.position;
        maxBottomSpawnPoint = GameObject.Find("PointMaxBottom-L").transform.position;
        enemyPrefab = enemyPrefabList[0];
        boss = GameObject.Find("EnemyBossTest").GetComponent<Boss>();
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     StartCoroutine(SpawnEnemy());
        // }
    }

    public IEnumerator SpawnEnemy() {
        //Here we calculate how many segments will fit between the two points based on spawn limit and cost, with max segments of 10
        segmentsToCreate = (spawnLimitCost / enemyPrefab.spawnCost);
        if (segmentsToCreate > 10) segmentsToCreate = 10;
        //As we'll be using vector3.lerp we want a value between 0 and 1, and the distance value is the value we have to add
        distance = ((float)1 / segmentsToCreate);

        while (spawnLimitCost > 0 && enemyInScene < maxEnemyLimit) {

            if (!boss.canShoot) { yield break; }

            //We increase our lerpValue
            lerpValue += distance;

            //Check for 1st segment and set position
            if (isFirstSegment) { unadjustedStartPosition = maxTopSpawnPoint; isFirstSegment = false; }
            else { unadjustedStartPosition = newUnadjustedStartPosition; }

            //Get the target position after spawn
            unadjustedEndPosition = Vector3.Lerp(maxTopSpawnPoint, maxBottomSpawnPoint, lerpValue);
            newUnadjustedStartPosition = unadjustedEndPosition;
            targetPosition = (unadjustedStartPosition + unadjustedEndPosition)/2;

            //Instantiate the object
            Enemy newEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            enemiesList.Add(newEnemy);
            yield return new WaitForSeconds(0.1f);
            spawnLimitCost -= newEnemy.spawnCost;
            enemyInScene += 1;
        }
        waveLimit -= 1;

        sharedRandomRange = Random.Range(waveDataPerLevelArray[currentLevel-1, 0], waveDataPerLevelArray[currentLevel-1, 1]);

        Invoke("ChangeEnemyStateToMoving", 2f);
        Invoke("NextWave", 3f);
    }

    private void ChangeEnemyStateToMoving() {
        foreach(Enemy enemies in enemiesList) {
            if (enemies.enemyState == EnemyState.SettingUp) {
                if (enemies) {
                    enemies.RemoveShield();
                    enemies.SetEnemyState(EnemyState.Moving);
                }
            }
        }
    }

    private void NextWave() {
        if(waveLimit > 0) {
            //reset value
            lerpValue = 0;
            isFirstSegment = true;
            enemyInScene = 0;
            spawnLimitCost = 10;
            if (currentLevel >= 4) { spawnLimitCost = 15; }
            // spawnLimitCost = (currentLevel * 5) + 5;
            currentWave += 1;
            enemyPrefab = enemyPrefabList[sharedRandomRange];

            StartCoroutine(SpawnEnemy());
        }
    }

    public void NextLevel() {
        currentLevel += 1;
        waveLimit = 4 + currentLevel;
        //reset value
        lerpValue = 0;
        isFirstSegment = true;
        enemyInScene = 0;
        spawnLimitCost = 10;
        if (currentLevel >= 6) { spawnLimitCost = 15; }

        enemyPrefab = enemyPrefabList[sharedRandomRange];
        StartCoroutine(SpawnEnemy());
    }

    public void StopSpawning() {
        StopCoroutine(SpawnEnemy());
    }
}
