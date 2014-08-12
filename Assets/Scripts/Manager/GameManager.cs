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
    public const string versionID = "Release 1.00";
    public const float DEFAULT_SENSITIVITY = 0.1f;
    public const int MAX_LIVES = 3;

    public static float mouseSensitivity = DEFAULT_SENSITIVITY;
    public static float effectsVolume = 1.0f;
    public static float musicVolume = 1.0f;
    public static bool invertedJoystick = false;
    public static bool invertedControls = false;

    private static GameManager _instance;

    public Texture2D _livesIcon;
    public List<GameObject> levels;
    public GameObject playerPrefab;
    public GameObject joystickLeft;
    public GameObject joystickRight;
    public bool enableSeebright = false;
    public bool isMenu = false;

    private SeebrightSDK _seebrightSDK;
    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = MAX_LIVES;
    private int _currentLevelIndex = 0;
    private bool _playingGame = false;
    private List<Player> _currentPlayerShips;
    private GameObject _currentLevelObject;
    private Level _currentLevel;
    private float _gameOverStartTimer;
    private bool _needToInitSeebrightCamera = false;
    private CNJoystick _joystickLeftAPI;
    private CNJoystick _joystickRightAPI;
    private Vector3 _joystickMovementVector;
    private TriggerButton _triggerButton;
    private bool _gameOver = false;
    private AudioSource _music;

    void Start () {
        //Implements Singleton 
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
            return;
        }
        _currentPlayerShips = new List<Player>();
        _instance = this;
        _needToInitSeebrightCamera = gameObject.GetComponent<SeebrightSDK>().enabled = enableSeebright;
#if UNITY_IPHONE
        if (!enableSeebright && !isMenu) {
            if (joystickLeft != null) {
                joystickLeft.SetActive(true);
                _joystickLeftAPI = joystickLeft.GetComponentInChildren<CNJoystick>();
                _joystickLeftAPI.JoystickMovedEvent += _joystickAPI_JoystickMovedEvent;
                _joystickLeftAPI.FingerLiftedEvent += _joystickAPI_FingerLiftedEvent;
                joystickLeft.SetActive(invertedControls);
            }
            if (joystickRight != null) {
                joystickRight.SetActive(true);
                _joystickRightAPI = joystickRight.GetComponentInChildren<CNJoystick>();
                _joystickRightAPI.JoystickMovedEvent += _joystickAPI_JoystickMovedEvent;
                _joystickRightAPI.FingerLiftedEvent += _joystickAPI_FingerLiftedEvent;
                joystickRight.SetActive(!invertedControls);
            }
            _triggerButton = GetComponent<TriggerButton>();
            if (_triggerButton != null) {
                _triggerButton.enabled = true;
            } else {
                Debug.LogError("No trigger button for iOS build!");
            }
        }
#endif
    }

    public bool isGameOver {
        get {
            return _gameOver;
        }

        set {
            _gameOver = value;
        }
    }

#if UNITY_IPHONE
    public bool TriggerPressed {
        get {
            return _triggerButton.Pressed;
        }
    }

    public float JoystickHorizontal {
        get {
            return _joystickMovementVector.x;
        }
    }

    public float JoystickVertical {
        get {
            return _joystickMovementVector.y;
        }
    }

    /**
     * Clears movement vector when finger is lifted
     */
    void _joystickAPI_FingerLiftedEvent() {
        _joystickMovementVector = Vector3.zero;
    }

    /**
     * Gets the joystick movement vector
     */
    void _joystickAPI_JoystickMovedEvent(Vector3 relativeVector) {
        _joystickMovementVector = relativeVector;
    }
#endif

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
#if !UNITY_IPHONE
        Screen.lockCursor = true;
#endif
        GameObject musicObject = GameObject.Find("IngameMusic");
        if (musicObject != null) {
            _music = musicObject.GetComponent<AudioSource>();
            updateMusicVolume();
            _music.Play();
        }
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
            if (GameManager.Instance.enableSeebright)
            {
                GUIManager.Instance.addGUIItem(new GUIItem(3 * ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(200), "Level Begin", tempStyle, 2));
            }
        } else {
            //Restarts multiplier
            Score.CurrentMultiplier = 0;
            Score.BuildUp = 0;
            GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
            tempStyle.normal.textColor = Color.red;
            tempStyle.alignment = TextAnchor.MiddleCenter;
            //Prints message when ship destroyed
            GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(200), "Ship Destroyed", tempStyle, 2));
            if (GameManager.Instance.enableSeebright)
            {
                GUIManager.Instance.addGUIItem(new GUIItem(3 * ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(200), "Ship Destroyed", tempStyle, 2));
            }
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
        isGameOver = true;
        _gameOverStartTimer = Time.time;
        Screen.lockCursor = false;
        GUIManager.Instance.clearGUIItem();
        GUIStyle tempStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.normal.textColor = Color.red;
        GUIManager.Instance.addGUIItem(new GUIItem(ScreenUtil.ScreenWidth / 2, ScreenUtil.ScreenHeight / 2, "Game Over!", tempStyle));
        if (GameManager.Instance.enableSeebright)
        {
            GUIManager.Instance.addGUIItem(new GUIItem(3 * ScreenUtil.ScreenWidth / 2, ScreenUtil.ScreenHeight / 2, "Game Over!", tempStyle));
        }
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
            if (GameManager.Instance.enableSeebright)
            {
                GUIManager.Instance.addGUIItem(new GUIItem(3 * ScreenUtil.ScreenWidth / 2, ScreenUtil.getPixelHeight(150),
                    "Gained Additional Ship!", tempStyle, 3));
            }
            _lives++;
        }
        _currentLevelIndex++;
        Time.timeScale = 1.0f + 0.02f * _currentLevelIndex;
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

    public int NumberOfLevels {
        get {
            return levels.Count;
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

    public void updateMusicVolume() {
        if (_music != null) {
            _music.volume = musicVolume;
        }
    }

    /*
     * Update function that runs every frame; Called within Unity
     */
    void Update () {
#if UNITY_IPHONE
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            _joystickMovementVector = Vector3.zero;
        }
#endif
        if(_needToInitSeebrightCamera && enableSeebright) {
            initSeebright();
        }
        //Goes back to main menu if game over
        if (isGameOver) {
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
            if (GameManager.Instance.enableSeebright)
            {
                GUI.DrawTexture(new Rect(ScreenUtil.ScreenWidth - ScreenUtil.getPixelHeight(_livesIcon.width) * (i + 1) + ScreenUtil.ScreenWidth, topBuffer,
                    ScreenUtil.getPixelHeight(_livesIcon.width), ScreenUtil.getPixelHeight(_livesIcon.height)), _livesIcon);
            }
        }
    }
}
