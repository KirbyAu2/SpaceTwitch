using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public const float DEFAULT_SENSITIVITY = 0.1f;
    public const int MAX_LIVES = 3;

    private static GameManager _instance;

    public Texture2D _livesIcon;
    public List<GameObject> levels;
    public GameObject playerPrefab;

    public static float mouseSensitivity = DEFAULT_SENSITIVITY;

    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = MAX_LIVES;
    private int _currentLevelIndex = 0;
    private bool _playingGame = false;
    private List<Player> _currentPlayerShips;
    private GameObject _currentLevelObject;
    private Level _currentLevel;
    private bool _gameOver = false;
    private float _gameOverStartTimer;

    void Start () {
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
            return;
        }
        _currentPlayerShips = new List<Player>();
        _instance = this;
    }

    public void StartGame() {
        _playingGame = true;
        _currentLevelIndex--;
        loadNextLevel();
        spawnPlayer(true);
        Screen.lockCursor = true;
    }

    public void spawnPlayer(bool firstTime = false) {
        if (firstTime) {
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.alignment = TextAnchor.MiddleCenter;
            GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(200), "Level Begin", tempStyle, 2));
        } else {
            Score.CurrentMultiplier = 0;
            Score.BuildUp = 0;
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.normal.textColor = Color.red;
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
        Instantiate(playerPrefab);
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
        Score.submit();
        CameraController.currentCamera.gameObject.GetComponent<BlurEffect>().enabled = true;
        _gameOver = true;
        _gameOverStartTimer = Time.time;
        Screen.lockCursor = false;
        GUIManager.Instance.clearGUIItem();
        GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.normal.textColor = Color.red;
        GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, Screen.height / 2, "Game Over!", tempStyle));
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
        if (_lives < MAX_LIVES) {
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.alignment = TextAnchor.MiddleCenter;
            tempStyle.normal.textColor = Color.green;
            GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(150), "Gained Additional Ship!", tempStyle, 3));
            _lives++;
        }
        _currentLevelIndex++;
        Time.timeScale = 1.0f + 0.1f * _currentLevelIndex;

        Vector3 newLevelPosition = 
            new Vector3((_currentLevelIndex+1) * levels[_currentLevelIndex % levels.Count].gameObject.renderer.bounds.size.x * -6.0f,0,0);
        _currentLevelObject = (GameObject)Instantiate(levels[_currentLevelIndex % levels.Count], newLevelPosition,
                                                      Quaternion.Euler(-90,0,0));
        _currentLevel = _currentLevelObject.GetComponent<Level>();
        _currentLevel.load();
        EnemyManager.Instance.loadLevel();
        foreach (Player s in _currentPlayerShips) {
            s.loadNextLevel(_currentLevel);
        }
    }

    public void winGame() {
        _playingGame = false;
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
        if (_gameOver) {
            if ((Time.time - _gameOverStartTimer) / 3.0f > 1.0f) {
                Application.LoadLevel(0);
            }
            return;
        }
        if (!_playingGame) {
            if (EnemyManager.Instance != null) {
                StartGame();
                return;
            }
        }
        if (CurrentLevel.isTutorial) {
            _lives = MAX_LIVES;
        }
    }

    void OnGUI() {
        for (int i = 0; i < _lives; i++) {
            GUI.DrawTexture(new Rect(Screen.width - ScreenUtil.getPixels(_livesIcon.width) * (i + 1), 0, 
                ScreenUtil.getPixels(_livesIcon.width), ScreenUtil.getPixels(_livesIcon.height)), _livesIcon);
        }
    }
}
