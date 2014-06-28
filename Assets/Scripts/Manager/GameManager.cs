using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    public List<Level> levels;

    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = 0;
    private int _currentLevel = 0;
    private bool _playingGame = false;

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
        loadNextLevel();
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
