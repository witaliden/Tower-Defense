using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Tworzenie singletona LevelManager
    private static LevelManager instance = null;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LevelManager>();
            }
            return instance;
        }
    }

    [SerializeField] private Transform towerUIParent;
    [SerializeField] private GameObject towerUIPrefab;
    [SerializeField] private Tower[] towerPrefabs;

    private List<Tower> spawnedTowers = new List<Tower>();

    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private Transform[] enemyPaths;
    [SerializeField] private float spawnDelay = 5f;

    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private float runningSpawnDelay;

    private List<Bullet> spawnedBullets = new List<Bullet>();

    public bool IsOver { get; private set; }

    [SerializeField] private int maxLives = 3;
    [SerializeField] private int totalEnemy = 15;

    [SerializeField] private GameObject panel;
    [SerializeField] private Text statusInfo;
    [SerializeField] private Text livesInfo;
    [SerializeField] private Text totalEnemyInfo;

    private int currentLives;
    private int enemyCounter;

    // Start jest wywoływany przed pierwszym Update
    private void Start()
    {
        SetCurrentLives(maxLives);
        SetTotalEnemy(totalEnemy);
        InstantiateAllTowerUI();
    }

    // Update jest wywoływany co kratkę
    private void Update()
    {
        // Restart gry po naciśnięciu klawisza R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (IsOver)
        {
            return;
        }

        // Licznik spawnu przeciwników
        // Time.unscaledDeltaTime to niezależny czas gry od czasu rzeczywistego
        runningSpawnDelay -= Time.unscaledDeltaTime;
        if (runningSpawnDelay <= 0f)
        {
            SpawnEnemy();
            runningSpawnDelay = spawnDelay;
        }

        foreach (Tower tower in spawnedTowers)
        {
            tower.CheckNearestEnemy(spawnedEnemies);
            tower.SeekTarget();
            tower.ShootTarget();
        }

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }

            if (Vector2.Distance(enemy.transform.position, enemy.TargetPosition) < 0.1f)
            {
                enemy.SetCurrentPathIndex(enemy.CurrentPathIndex + 1);
                if (enemy.CurrentPathIndex < enemyPaths.Length)
                {
                    enemy.SetTargetPosition(enemyPaths[enemy.CurrentPathIndex].position);
                }
                else
                {
                    ReduceLives(1);
                    enemy.gameObject.SetActive(false);
                }
            }

            else
            {
                enemy.MoveToTarget();
            }
        }
    }

    // Inicjalizacja wszystkich wież
    private void InstantiateAllTowerUI()
    {
        foreach (Tower tower in towerPrefabs)
        {
            GameObject newTowerUIObj = Instantiate(towerUIPrefab.gameObject, towerUIParent);
            TowerUI newTowerUI = newTowerUIObj.GetComponent<TowerUI>();
            newTowerUI.SetTowerPrefab(tower);
            newTowerUI.transform.name = tower.name;
        }
    }

    // Dodanie nowej wieży do managera poziomów
    public void RegisterSpawnedTower(Tower tower)
    {
        spawnedTowers.Add(tower);
    }

    private void SpawnEnemy()
    {
        SetTotalEnemy(--enemyCounter);
        if (enemyCounter < 0)
        {
            bool isAllEnemyDestroyed = spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;
            if (isAllEnemyDestroyed)
            {
                SetGameOver(true);
            }
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        string enemyIndexString = (randomIndex + 1).ToString();
        GameObject newEnemyObj = spawnedEnemies.Find(e => !e.gameObject.activeSelf && e.name.Contains(enemyIndexString))?.gameObject;
        if (newEnemyObj == null)
        {
            newEnemyObj = Instantiate(enemyPrefabs[randomIndex].gameObject);
        }

        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
        if (!spawnedEnemies.Contains(newEnemy))
        {
            spawnedEnemies.Add(newEnemy);
        }

        newEnemy.transform.position = enemyPaths[0].position;
        newEnemy.SetTargetPosition(enemyPaths[1].position);
        newEnemy.SetCurrentPathIndex(1);
        newEnemy.gameObject.SetActive(true);
    }

    // Rysuje linie między punktami ścieżki przeciwników
    private void OnDrawGizmos()
    {
        for (int i = 0; i < enemyPaths.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(enemyPaths[i].position, enemyPaths[i + 1].position);
        }
    }

    public Bullet GetBulletFromPool(Bullet prefab)
    {
        GameObject newBulletObj = spawnedBullets.Find(b => !b.gameObject.activeSelf && b.name.Contains(prefab.name))?.gameObject;
        if (newBulletObj == null)
        {
            newBulletObj = Instantiate(prefab.gameObject);
        }

        Bullet newBullet = newBulletObj.GetComponent<Bullet>();
        if (!spawnedBullets.Contains(newBullet))
        {
            spawnedBullets.Add(newBullet);
        }

        return newBullet;
    }

    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy.gameObject.activeSelf && Vector2.Distance(enemy.transform.position, point) <= radius)
            {
                enemy.ReduceEnemyHealth(damage);

            }
        }
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(currentLives - value);
        if (currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    public void SetCurrentLives(int currentLives)
    {
        this.currentLives = Mathf.Max(currentLives, 0);
        livesInfo.text = $"Życia: {this.currentLives}";
    }

    public void SetTotalEnemy(int totalEnemy)
    {
        enemyCounter = totalEnemy;
        totalEnemyInfo.text = $"Pozostało wrogów: {Mathf.Max(enemyCounter, 0)}";
    }

    public void SetGameOver(bool isWin)
    {
        IsOver = true;
        statusInfo.text = isWin ? "Wygrane!" : "Przegrane!";
        panel.SetActive(true);
    }
}
