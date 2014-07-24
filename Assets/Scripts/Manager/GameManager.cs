using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The GameManager class manages the game
 * GameManager starts the game and go back to main menu when game over
 * Manages levels completion and loads new level
 * Keeps track of player lives until game over
 */
public class GameManager : MonoBehaviour {
    public const float DEFAULT_SENSITIVITY = 0.1f;
    public const int MAX_LIVES = 3;

    private static GameManager _instance;
    private AudioSource _music;

    public Texture2D _livesIcon;
    public List<GameObject> levels;
    public GameObject playerPrefab;
    public bool enableSeebright = false;
    public bool motionEnabled = false;
    public bool isMenu = false;

    public static float mouseSensitivity = DEFAULT_SENSITIVITY;
    public static float effectsVolume = 1.0f, musicVolume = 1.0f;

    private SeebrightSDK _seebrightSDK;
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
    private bool _needToInitSeebrightCamera = false;

    void Start () {
        //Implements Singleton 
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
            return;
        }
        _needToInitSeebrightCamera = enableSeebright;
        _currentPlayerShips = new List<Player>();
        _instance = this;
    }


    /**
     * Handles the initialization of all things Seebright
     */
    private void initSeebright() {
        if(CameraController.currentCamera != null) {
            CameraController.currentCamera.initSeebright();
            _needToInitSeebrightCamera = false;
        } else {
            _needToInitSeebrightCamera = true;
            return;
        }
        _seebrightSDK = GetComponentInChildren<SeebrightSDK>();
    }

    //Starts the game
    public void StartGame() {
        _playingGame = true;
        _currentLevelIndex--;
        loadNextLevel();
        spawnPlayer(true);
        Screen.lockCursor = true;
        GameObject musicObject = GameObject.Find("IngameMusic");
        if (musicObject != null)
            _music = musicObject.audio;
    }

    /*
     * Spawns player ship
     * Draws GUI messages 
     * Reinitializes multiplier
     * Check lives
     */
    public void spawnPlayer(bool firstTime = false) {
        if (firstTime) {
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.alignment = TextAnchor.MiddleCenter;
            //Prints message if new game
            GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(200), "Level Begin", tempStyle, 2));
        } else {
            //Restarts multiplier
            Score.CurrentMultiplier = 0;
            Score.BuildUp = 0;
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.normal.textColor = Color.red;
            tempStyle.alignment = TextAnchor.MiddleCenter;
            //Prints message when ship destroyed
            GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(200), "Ship Destroyed", tempStyle, 2));
        }
        _currentPlayerShips.Clear();
        //If stil have lives, player loses one life, else game over
        if (!firstTime && _lives > 0) {
            _lives--;
        } else if (_lives < 1) {
            gameOver();
            return;
        }
        Instantiate(playerPrefab);
    }

    //Add player ship
    public void addShip(Player s) {
        _currentPlayerShips.Add(s);
        s.init(_currentLevel);
    }

    //Removes player ship; if only one, then call to spawn new player ship
    public void removeShip(Player s) {
        _currentPlayerShips.Remove(s);
        if(_currentPlayerShips.Count < 1) {
            spawnPlayer();
        }
    }

    /*
     * When game over, submit score and print gui mesage
     */
    public void gameOver() {
        Score.submit();
        CameraController.currentCamera.setBlurShader(true);
        _gameOver = true;
        _gameOverStartTimer = Time.time;
        Screen.lockCursor = false;
        GUIManager.Instance.clearGUIItem();
        GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.normal.textColor = Color.red;
        GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.ScreenHeight / 2, "Game Over!", tempStyle));
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
        //Player games one life
        if (_lives < MAX_LIVES) {
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.alignment = TextAnchor.MiddleCenter;
            tempStyle.normal.textColor = Color.green;
            GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(150), 
                "Gained Additional Ship!", tempStyle, 3));
            _lives++;
        }
        _currentLevelIndex++;
        Time.timeScale = 1.0f + 0.1f * _currentLevelIndex;
        //Loads new level
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

    /**
     * Updates the music for the game
     */
    public void UpdateMusicVolume()
    {
        if (_music != null) { 
            _music.volume = musicVolume;
        }
    }

    /*
     * Update function that runs every frame; Called within Unity
     */
    void Update () {
        if(_needToInitSeebrightCamera && enableSeebright) {
            initSeebright();
        }
        //Goes back to main menu if game over
        if (_gameOver) {
            if ((Time.time - _gameOverStartTimer) / 3.0f > 1.0f) {
                Application.LoadLevel(0);
            }
            return;
        }
        //Calls to start game
        if (!_playingGame) {
            if (EnemyManager.Instance != null) {
                StartGame();
                return;
            }
        }
        if(isMenu) {
            return;
        }
        if (CurrentLevel.isTutorial) {
            _lives = MAX_LIVES;
        }
    }

    /*
     * OnGUI is called for rendering and handlng GUI events 
     * Draws the lives icon 
     */
    void OnGUI() {
        if(_livesIcon == null) {
            return;
        }
        //A buffer to add in case seebright is enabled
        float topBuffer = GameManager.Instance.enableSeebright ? ScreenUtil.getPixelHeight(50) : 0;
        for (int i = 0; i < _lives; i++) {
            GUI.DrawTexture(new Rect(ScreenUtil.ScreenWidth - ScreenUtil.getPixelHeight(_livesIcon.width) * (i + 1), topBuffer, 
                ScreenUtil.getPixelHeight(_livesIcon.width), ScreenUtil.getPixelHeight(_livesIcon.height)), _livesIcon);
        }
    }
}
