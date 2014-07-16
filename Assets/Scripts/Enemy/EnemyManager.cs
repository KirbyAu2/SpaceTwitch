using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
    private const float STANDARD_SPAWN_TIME = 1.0f;
    private const float PAWN_SPAWN_TIME = 0.0f;
    private const float CROSSHATCH_SPAWN_TIME = 5.0f;
    private const float SWIRLIE_SPAWN_TIME = 1.0f;
    private const float CONFETTI_SPAWN_TIME = 1.5f;
    private const float DECREMENT_FOR_DIFFICULTY = 0.04f;
    private const float LOWEST_SPAWN_TIME = 0.5f;

    private static EnemyManager _instance;

    public GameObject pawnPrefab;
    public GameObject crosshatchPrefab;
    public GameObject swirliePrefab;
    public GameObject confettiPrefab;
    public GameObject enemyProjectilePrefab;
    public GameObject powerupPrefab;

    private List<string> _potentialEnemies;
    private List<Enemy> _currentEnemies;
    private string prevEnemyID;
    private Level _currentLevel;
    private float _nextSpawnTime;

    void Start () {
        if(_instance != null) {
            Debug.LogError("There can't be two Enemy Managers!");
        }
        _currentEnemies = new List<Enemy>();
        _instance = this;
        prevEnemyID = Level.ID_PAWN;
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

    /*
     * Removes enemy
     */
    public void removeEnemy(Enemy e) {
        _currentEnemies.Remove(e);
    }

    /*
     * Checks and Spawns enemies 
     */
    private void spawnEnemy() {
        //Return if in tutorial and not ready to spawn
        if (_currentLevel.isTutorial) {
            if (!_currentLevel.Tutorial.readyToSpawn) {
                return;
            }
        }
        //Return if no potential enemies
        if (_potentialEnemies == null) {
            return;
        }
        //Return if level is transitioning
        if (GameManager.Instance.CurrentPlayerShips.Count > 0) {
            if (GameManager.Instance.CurrentPlayerShips[0].isTransitioning) {
                return;
            }
        }
        //Load next level if no more enemies
        if (!_currentLevel.isTutorial) {
            if (_potentialEnemies.Count == 0 && _currentEnemies.Count == 0) {
                GameManager.Instance.loadNextLevel();
                return;
            } else if (_potentialEnemies.Count == 0) { //Return if no more potential enemies
                return;
            }
        } else {
            //End tutorial if no more enemies in tutorial
            if (_potentialEnemies.Count == 0 && _currentEnemies.Count == 0) {
                _currentLevel.Tutorial.endTutorial();
                return;
            }
        }
        string nextEnemyID = _potentialEnemies[0]; //what is the next enemy
        if (prevEnemyID != null && _currentLevel.isTutorial) {
            if (prevEnemyID != nextEnemyID && _currentEnemies.Count > 0) {
                return;
            } else if(prevEnemyID != nextEnemyID) {
                prevEnemyID = nextEnemyID;
                _currentLevel.Tutorial.displayNext();
            }
        }
        _potentialEnemies.RemoveAt(0);
        GameObject g;
        _nextSpawnTime = STANDARD_SPAWN_TIME;
        
        //Spawns Enemies 
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
        _nextSpawnTime -= (GameManager.Instance.CurrentDifficulty * DECREMENT_FOR_DIFFICULTY);
        if(_nextSpawnTime < LOWEST_SPAWN_TIME) {
            _nextSpawnTime = LOWEST_SPAWN_TIME;
        }
        _nextSpawnTime += Time.time;

        _currentEnemies[_currentEnemies.Count - 1].spawn(_currentLevel.getRandomLane());
    }

    /*
     * Calls to spawn Enemies when time 
     */
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
