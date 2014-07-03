using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
    private const float STANDARD_SPAWN_TIME = 1.0f;
    private const float PAWN_SPAWN_TIME = 0.0f;
    private const float CROSSHATCH_SPAWN_TIME = 5.0f;
    private const float SWIRLIE_SPAWN_TIME = 1.0f;
    private const float CONFETTI_SPAWN_TIME = 5.0f;

    private static EnemyManager _instance;

    public GameObject pawnPrefab;
    public GameObject crosshatchPrefab;
    public GameObject swirliePrefab;
    public GameObject confettiPrefab;
    public GameObject enemyProjectilePrefab;

    private List<string> _potentialEnemies;
    private List<Enemy> _currentEnemies;

    private Level _currentLevel;
    private float _nextSpawnTime;

    void Start () {
        if(_instance != null) {
            Debug.LogError("There can't be two Enemy Managers!");
        }
        _currentEnemies = new List<Enemy>();
        _instance = this;
        DontDestroyOnLoad(this);
    }

    /**
     * Call this whenever a new level is loaded or restarted
     */
    public void loadLevel() {
        if (_potentialEnemies != null) {
            _potentialEnemies.Clear();
        }
        if (_currentEnemies != null) {
            _currentEnemies.Clear();
        }

        _currentLevel = GameManager.Instance.CurrentLevel;
        _potentialEnemies = _currentLevel.PotentialEnemies;
        _nextSpawnTime = Time.time;
    }

    public static EnemyManager Instance {
        get { 
            return _instance;
        }
    }

    public void removeEnemy(Enemy e) {
        _currentEnemies.Remove(e);
    }

    private void spawnEnemy() {
        if (_potentialEnemies == null) {
            return;
        }
        if (GameManager.Instance.CurrentPlayerShips.Count > 0) {
            if (GameManager.Instance.CurrentPlayerShips[0].isTransitioning) {
                return;
            }
        }
        if (_potentialEnemies.Count == 0 && _currentEnemies.Count == 0) {
            GameManager.Instance.loadNextLevel();
            Debug.Log("All out of enemies!");
            return;
        } else if (_potentialEnemies.Count == 0) {
            return;
        }
        string nextEnemyID = _potentialEnemies[0];
        _potentialEnemies.RemoveAt(0);
        GameObject g;
        _nextSpawnTime = Time.time + STANDARD_SPAWN_TIME;
        switch (nextEnemyID) {
            case Level.ID_PAWN:
                g = (GameObject)Instantiate(pawnPrefab);
                _currentEnemies.Add(g.GetComponent<Enemy>());
                _nextSpawnTime += PAWN_SPAWN_TIME;
                break;

            case Level.ID_CONFETTI:
                g = (GameObject)Instantiate(confettiPrefab);
                _currentEnemies.Add(g.GetComponent<Enemy>());
                _nextSpawnTime += CONFETTI_SPAWN_TIME;
                break;

            case Level.ID_CROSSHATCH:
                g = (GameObject)Instantiate(crosshatchPrefab);
                _currentEnemies.Add(g.GetComponent<Enemy>());
                _nextSpawnTime += CROSSHATCH_SPAWN_TIME;
                break;

            case Level.ID_SWIRLIE:
                g = (GameObject)Instantiate(swirliePrefab);
                _currentEnemies.Add(g.GetComponent<Enemy>());
                _nextSpawnTime += SWIRLIE_SPAWN_TIME;
                break;
        }
        _currentEnemies[_currentEnemies.Count - 1].spawn(_currentLevel.getRandomLane());
    }

    void Update () {
        if (GameManager.Instance != null) {
            if (!GameManager.Instance.PlayingGame) {
                return;
            }
            if (Time.time > _nextSpawnTime) {
                spawnEnemy();
            }
        }
    }
}
