using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    public List<GameObject> levels;
    public GameObject playerPrefab;

    public float mouseSensitivity;

    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = 3;
    private int _currentLevelIndex = 0;
    private bool _playingGame = false;
    private List<Player> _currentPlayerShips;
    private GameObject _currentLevelObject;
    private Level _currentLevel;

    void Start () {
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
            return;
        }
        _currentPlayerShips = new List<Player>();
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        _playingGame = true;
        _currentLevelIndex--;
        loadNextLevel();
        spawnPlayer(true);
    }

    public void spawnPlayer(bool firstTime = false) {
        if (firstTime) {
            GUIStyle tempStyle = GUIManager.Instance.defaultStyle;
            tempStyle.alignment = TextAnchor.MiddleCenter;
            GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(200), "Level Begin", tempStyle, 2));
        } else {
            GUIStyle tempStyle = GUIManager.Instance.defaultStyle;
            tempStyle.alignment = TextAnchor.MiddleCenter;
            GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(200), "Ship Destroyed", tempStyle, 2));
        }
        _currentPlayerShips.Clear();
        if (!firstTime && _lives > 0) {
            _lives--;
        } else if (_lives < 1) {
            gameOver();
            return;
        }
        GameObject currentPlayerShip = (GameObject)Instantiate(playerPrefab);
    }

    public void addShip(Player s) {
        _currentPlayerShips.Add(s);
        s.init(_currentLevel);
    }

    public void removeShip(Player s) {
        _currentPlayerShips.Remove(s);
        if(_currentPlayerShips.Count < 1) {
            spawnPlayer();
        }
    }

    public void gameOver() {
        GUIManager.Instance.clearGUIItem();
        GUIStyle tempStyle = GUIManager.Instance.defaultStyle;
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.normal.textColor = Color.red;
        GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(200), "Game Over!", tempStyle));
    }

    public List<Player> CurrentPlayerShips {
        get {
            return _currentPlayerShips;
        }
    }

    /**
     * Loads the next level
     */
    public void loadNextLevel() {
        _currentLevelIndex++;
        Vector3 newLevelPosition = 
            new Vector3((_currentLevelIndex+1) * levels[_currentLevelIndex].gameObject.renderer.bounds.size.x * -3.0f,0,0);
        _currentLevelObject = (GameObject)Instantiate(levels[_currentLevelIndex],newLevelPosition,
                                                      Quaternion.Euler(-90,0,0));
        _currentLevel = _currentLevelObject.GetComponent<Level>();
        _currentLevel.load();
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
            return _currentLevelIndex + 1;
        }
    }

    public Level CurrentLevel {
        get {
            return _currentLevel;
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
