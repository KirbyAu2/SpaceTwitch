using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    public List<Level> levels;
    public GameObject playerPrefab;

    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = 3;
    private int _currentLevel = 0;
    private bool _playingGame = false;
    private GameObject _currentPlayerShip;
    private Player _currentPlayerScript;

    void Start () {
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        _playingGame = true;
        _currentLevel--;
        //spawnPlayer(true);
        loadNextLevel();
    }

    public void spawnPlayer(bool firstTime = false) {
        if (!firstTime && _lives > 0) {
            _lives--;
        } else if (_lives < 1) {
            gameOver();
            return;
        }
        _currentPlayerShip = (GameObject)Instantiate(playerPrefab);
        _currentPlayerScript = _currentPlayerShip.GetComponent<Player>();
    }

    public void gameOver() {

    }

    /**
     * Loads the next level
     */
    public void loadNextLevel() {
        _currentLevel++;
        levels[_currentLevel].load();
        EnemyManager.Instance.loadLevel();
    }

    public void stopGame() {
        _playingGame = false;
    }

    public bool PlayingGame {
        get {
            return _playingGame;
        }
    }

    public static GameManager Instance {
        get {
            return _instance;
        }
    }

    public int CurrentDifficulty {
        get {
            return _currentLevel + 1;
        }
    }

    public Level CurrentLevel {
        get {
            return levels[_currentLevel];
        }
    }

    public int CurrentScore {
        get {
            return _score;
        }
    }

    public int Multiplier {
        get {
            return _multiplier;
        }
    }

    public int Lives {
        get {
            return _lives;
        }
    }

    void Update () {
        if (!_playingGame) {
            if (EnemyManager.Instance != null) {
                StartGame();
            }
        }
    }
}
