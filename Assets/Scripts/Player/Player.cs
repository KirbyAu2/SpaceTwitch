﻿using UnityEngine;
using System.Collections;

/*
 * Player Class contains player shooting, power-ups, transistions, and player ship initializations.
 * The player ship shoots projectiles that collides with the enemy
 * The power ups will help the player by increasing the shot speed, making more shots, or making a second clone ship that the player can control
 * When a level is complete, the player ship transistions to the next level. 
 */
public class Player : MonoBehaviour {
    public const float RESPAWN_COOLDOWN = 3.0f;
    public const float PLAYER_LEVEL_TRANSITION_TIME = 1.5f;
    public const float CAMERA_LEVEL_TRANSITION_TIME = 1.75f;
    public const float FLASHBANG_TIME = 0.75f;
    public const float DELAY_NEXT_SHOT = .22f;
    public const float RAPID_SHOT_TIME = 5.0f;
    public const float MULTI_SHOT_TIME = 7.5f;
    public const int MAX_SHOTS = 20;
    public const float MOVE_DELAY = 0.08f;

    private const float CAMERA_PERCENT_BACK = .7f;
    private float _mouseSensitivity;

    public GameObject TestLevel;
    public Level currentLevel;

    public int currentPlane { get; private set; }
    public float positionOnPlane { get; private set; } // between 0 (beginning) and 1 (end)
    
    private bool _alive = false;
    private int _numShots = 0;
    private float _reload = 0;

    public GameObject playerProjectile;

    private float _rapidTime = 0;
    private float _multiTime = 0;

    public bool isRapidActivated { get; private set; }
    public bool isMultiActivated { get; private set; }
    public bool isCloneActivated { get; private set; }
    public bool isClone { get; private set; }// is this ship a clone?
    private bool _isMovementMirrored = false; // does the ship move in reverse?

    private Player _clone; // the player's clone
    public GameObject cloneObject; // the clone object

    private EscapeMenu _escapeMenu;

    //Flying variables
    private bool _transitioning = false;
    private float _startTransTime;
    private Vector3 _startPos;
    private Vector3 _cameraStartPos;
    private AudioClip _deathSound;
    private AudioClip _shootSound;
    private Flashbang _flashbang;
    private bool _invulnerable = false;
    private GameObject _previousLevel;
    private float _invulnerabilityCooldown;
    private bool _goingDownLane = false;
    private Vector3 _endOfLane;


    private float _moveTime = 0;

    //Initializes player ship
    void Start () {

        isRapidActivated = isMultiActivated = isCloneActivated = false;
        if (TestLevel != null) {
            currentLevel = TestLevel.GetComponent<Level>();
            init(currentLevel);
        }
        _mouseSensitivity = GameManager.mouseSensitivity;
        if (_mouseSensitivity < .1f) {
            _mouseSensitivity = .1f;
        }
        GameManager.Instance.addShip(this);
        gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _deathSound = (AudioClip)Resources.Load("Sound/ShipExplode");
        _shootSound = (AudioClip)Resources.Load("Sound/PlayerShoot");
    }


    /**
     * Updates the sensitivity
     * 
     */
    public void UpdateSensitivity()
    {
        _mouseSensitivity = GameManager.mouseSensitivity;
    }

    //During transition and loading next level
    public void loadNextLevel(Level level) {
        _transitioning = true;
        _goingDownLane = true;
        _invulnerable = false;
        _invulnerabilityCooldown = Time.time - RESPAWN_COOLDOWN;
        _previousLevel = currentLevel.gameObject;
        currentLevel.lanes[currentPlane].setHighlight(false);
        _startTransTime = Time.time;
        currentLevel = level;
        _startPos = gameObject.transform.position;
        _cameraStartPos = CameraController.currentCamera.gameObject.transform.position;
    }

    public bool isTransitioning {
        get {
            return _transitioning;
        }
    }

    //Initialize current level 
    public void init(Level level) {
        if (isClone) {
            return;
        }
        currentLevel = level;
        CameraController.currentCamera.gameObject.transform.position = currentLevel.cameraPosition.transform.position;
        currentPlane = currentLevel.lanes.IndexOf(currentLevel.SpawnLane);
        _alive = true;
        _invulnerable = true;
        _invulnerabilityCooldown = Time.time;
        _escapeMenu = gameObject.GetComponent<EscapeMenu>();
    }

    /**
     * Initializes the ship if it is a clone
     */
    public void initAsClone(Level level, int plane, float position) {
        isClone = true;
        currentLevel = level;
        _alive = true;
        // initialize clone location
        if (!currentLevel.wrapAround) { // level doesn't wrap
            _isMovementMirrored = true;
            currentPlane = level.lanes.Count - 1 - plane;
            positionOnPlane = 1 - position;
        }
        else {
            currentPlane = plane - (level.lanes.Count / 2);
            if (currentPlane < 0) {
                currentPlane += level.lanes.Count;
            }
            positionOnPlane = position;
        }
        _escapeMenu = GameManager.Instance.CurrentPlayerShips[0].GetComponent<EscapeMenu>();
    }

    public void handleMovement() {
#if UNITY_IPHONE && !UNITY_EDITOR
        if (_moveTime + MOVE_DELAY > Time.time) {
            return;
        }
        float mouseMove = (GameManager.Instance.enableSeebright) ? SBRemote.GetAxis(SBRemote.JOY_HORIZONTAL) : GameManager.Instance.JoystickHorizontal;
        positionOnPlane = mouseMove * 3;
        if (GameManager.invertedJoystick) {
            positionOnPlane *= -1;
        }
        if(_isMovementMirrored) {
            positionOnPlane *= -1;
        }
#elif UNITY_IPHONE && UNITY_EDITOR
        if (_moveTime + MOVE_DELAY > Time.time && !GameManager.Instance.enableSeebright) {
            return;
        }
        float mouseMove = (GameManager.Instance.enableSeebright) ? Input.GetAxis("Mouse X") : GameManager.Instance.JoystickHorizontal;
        if (GameManager.Instance.enableSeebright) {
            float shipMove = mouseMove * _mouseSensitivity;
            if (_isMovementMirrored) {
                positionOnPlane -= shipMove;
            } else {
                positionOnPlane += shipMove;
            }
        } else {
            positionOnPlane = mouseMove * 3;
            if (GameManager.invertedJoystick) {
                positionOnPlane *= -1;
            }
            if(_isMovementMirrored) {
                positionOnPlane *= -1;
            }
        }
#else
        float mouseMove = Input.GetAxis("Mouse X");
        float shipMove = mouseMove * _mouseSensitivity;
        if (_isMovementMirrored) {
            positionOnPlane -= shipMove;
        }
        else {
            positionOnPlane += shipMove;
        }
#endif

        currentLevel.lanes[currentPlane].setHighlight(false);
        // calculate new position after movement
        if (positionOnPlane < 0) {
            currentPlane--;
            positionOnPlane++;
        } else if (positionOnPlane > 1) {
            currentPlane++;
            positionOnPlane--;
        }

        if (currentPlane < 0) {
            if (currentLevel.wrapAround) {
                currentPlane += currentLevel.lanes.Count;
            } else {
                currentPlane = 0;
                positionOnPlane = 0;
            }
        } else if (currentPlane >= currentLevel.lanes.Count) {
            if (currentLevel.wrapAround) {
                currentPlane -= currentLevel.lanes.Count;
            } else {
                currentPlane = currentLevel.lanes.Count - 1;
                positionOnPlane = 1;
            }
        }
        _endOfLane = GameManager.Instance.CurrentLevel.getLaneFromIndex(currentPlane).Back;
        currentLevel.lanes[currentPlane].setHighlight(true);

        // update position
        transform.position = currentLevel.lanes[currentPlane].Front + new Vector3(renderer.bounds.extents.x, 0, 0);
        float angleUp = Vector3.Angle(Vector3.up, currentLevel.lanes[currentPlane].Normal) - 90;
        float angleRight = Vector3.Angle(Vector3.forward, currentLevel.lanes[currentPlane].Normal);
        float angleLeft = Vector3.Angle(Vector3.back, currentLevel.lanes[currentPlane].Normal);
        if (angleRight < angleLeft) {
            angleUp = -angleUp;
        }
        transform.eulerAngles = new Vector3(angleUp, 180, 0);
        _moveTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        //Escape Menu
#if UNITY_EDITOR
        bool escapeMenuKeyPressed = Input.GetKeyDown(KeyCode.Escape);
#else
        bool escapeMenuKeyPressed = GameManager.Instance.enableSeebright ? SBRemote.GetButton(SBRemote.BUTTON_BACK) : Input.GetKeyDown(KeyCode.Escape);
#endif
        if (escapeMenuKeyPressed && !isClone) {
            if (!_escapeMenu.currentlyActive) {
                _escapeMenu.display();
            }
        }
        if (_escapeMenu.currentlyActive) {
            return;
        }

#if !UNITY_IPHONE
        Screen.lockCursor = true;
#endif

        //Invulnerability
        if (_invulnerable) {
            renderer.enabled = Mathf.Sin(Time.time * 50.0f) > 0;
        }
        if (Time.time > _invulnerabilityCooldown + RESPAWN_COOLDOWN) {
            renderer.enabled = true;
            _invulnerable = false;
        }
        if (_transitioning && currentLevel.SpawnLane != null) {
            //if transitioning levels and clone is active, destroy clone
            if (isClone) {
                GameManager.Instance.removeShip(this);
                Destroy(gameObject);
            }
            setCamera();
            if (_goingDownLane) {
                gameObject.transform.position = Vector3.Lerp(_startPos, _endOfLane, (Time.time - _startTransTime) / (PLAYER_LEVEL_TRANSITION_TIME * 0.25f));
                if (gameObject.transform.position == _endOfLane) {
                    _goingDownLane = false;
                }
            } else {
                gameObject.transform.position = Vector3.Lerp(_endOfLane, currentLevel.SpawnLane.Front, (Time.time - _startTransTime) / (PLAYER_LEVEL_TRANSITION_TIME * 0.75f));
            }
            gameObject.transform.Rotate(new Vector3(1, 0, 0), 360.0f * (Time.time - _startTransTime) / PLAYER_LEVEL_TRANSITION_TIME);
            
            //Level Transition
            if (gameObject.transform.position == currentLevel.SpawnLane.Front && 
                CameraController.currentCamera.gameObject.transform.position == currentLevel.cameraPosition.transform.position) {
                    if (!_flashbang.running) {
                        _flashbang.init(FLASHBANG_TIME);
                    }
                    if ((Time.time - _startTransTime) / (PLAYER_LEVEL_TRANSITION_TIME + FLASHBANG_TIME / 2.0f) >= 1.0f) {
                        CameraController.currentCamera.setRetroShader(false);
                    }
                    if (_flashbang.manualUpdate()) {
                        return;
                    }
                CameraController.currentCamera.setRetroShader(false);
                _transitioning = false;
                    Destroy(_previousLevel);
                    if (currentLevel.SpawnLane != null) {
                        currentPlane = currentLevel.lanes.IndexOf(currentLevel.SpawnLane);
                    } else {
                        currentPlane = 0;
                    }
            }
            return;
        }
        if (currentLevel == null || currentLevel.lanes == null) {
            return;
        }

        //This is a preprocess directive, exclusively used with Unity Editor
        //#yoyoloop
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) { // testing purposes
            ActivateClone();
            ActivateMulti();
            ActivateRapid();
        }

        if (Input.GetKeyDown(KeyCode.F1)) {
            _invulnerable = !_invulnerable;
            _invulnerabilityCooldown = (_invulnerable) ? float.MaxValue : 0;
            GUIManager.Instance.addGUIItem(new GUIItem(Screen.width/2,Screen.height/2, "God Mode : " + _invulnerable.ToString(),GUIManager.Instance.defaultStyle,4));
        }
#endif

        if (!_alive) {
            return;
        }

        if (!isClone) {
            handleMovement();
            if (_clone) {
                _clone.handleMovement();
            }
        }

        // shoot
        if (_numShots < 0) { 
            _numShots = 0;
        }

        int maxMultiShots = MAX_SHOTS;
        if (isMultiActivated) {
            maxMultiShots = MAX_SHOTS * 3 - 2; // -2 for +3 shots from multi shot so it fires at 12 max (<13)
        }
#if UNITY_IPHONE && !UNITY_EDITOR
        bool triggerPressed = (GameManager.Instance.enableSeebright) ? SBRemote.GetButton(SBRemote.BUTTON_TRIGGER) : GameManager.Instance.TriggerPressed;
#elif UNITY_IPHONE && UNITY_EDITOR
        bool triggerPressed = (GameManager.Instance.enableSeebright) ? Input.GetMouseButton(0) : GameManager.Instance.TriggerPressed;
#else
        bool triggerPressed = Input.GetMouseButton(0);
#endif

        if (triggerPressed && _reload < 0 && _numShots < maxMultiShots) {
            Shoot();
            _reload = DELAY_NEXT_SHOT;
            if (isRapidActivated) {
                _reload /= 2.0f;
            }
        }
        
        // reload
        _reload -= Time.deltaTime;

        // powerup timers
        if (_rapidTime < 0) {
            isRapidActivated = false;
        }
        else {
            _rapidTime -= Time.deltaTime;
        }

        if (_multiTime < 0) {
            isMultiActivated = false;
        }
        else {
            _multiTime -= Time.deltaTime;
        }

        if (_clone == null) {
            isCloneActivated = false;
        }
    }

    //sets camera resolution
    private void setCamera() {
        if (_flashbang == null) {
            _flashbang = CameraController.currentCamera.gameObject.GetComponent<Flashbang>();
        }
        CameraController.currentCamera.setRetroShader(true);
        float percent = (Time.time - _startTransTime) / CAMERA_LEVEL_TRANSITION_TIME;

        CameraController.currentCamera.gameObject.transform.position = 
            Vector3.Lerp(_cameraStartPos, currentLevel.cameraPosition.transform.position,percent);
    }
    
    //gets current lane
    public Lane CurrentLane {
        get {
            if(currentLevel != null && currentLevel.lanes != null && currentPlane < currentLevel.lanes.Count) {
                return currentLevel.lanes[currentPlane];
            } else {
                return null;
            }
        }
    }

    /*
     * When player shoots, player projectile will shoot down lane that player was currently on when shot
     * Instantiates player projectile gameobject
     * If multi-shot is activated, player projectile will shoot down the lane left of current lane as well as the lane right of current lane
     */
    void Shoot() {
        AudioSource.PlayClipAtPoint(_shootSound, transform.position, GameManager.effectsVolume);
        Lane currentLane = currentLevel.lanes[currentPlane];
        GameObject shot = (GameObject)Instantiate(playerProjectile);
        shot.renderer.enabled = false;
        shot.GetComponent<PlayerProjectile>().init(currentLane, this);
        _numShots++;
        if (isMultiActivated) {
            currentLane = currentLevel.lanes[currentPlane].LeftLane;
            if (currentLane != null) {
                shot = (GameObject)Instantiate(playerProjectile);
                shot.GetComponent<PlayerProjectile>().init(currentLane, this);
                _numShots++;
            }
            currentLane = currentLevel.lanes[currentPlane].RightLane;
            if (currentLane != null) {
                shot = (GameObject)Instantiate(playerProjectile);
                shot.GetComponent<PlayerProjectile>().init(currentLane, this);
                _numShots++;
            }
        }
    }
    
    /*
     * Lowers shot count
     */
    public void RemoveShot() {
        _numShots--;
    }
    
    /*
     * If player ship dies, creates particle effects and sfx for death
     * Destroys gameobject and turns off plane highlight
     */
    void OnTriggerEnter(Collider other) {
        if (_invulnerable) {
            return;
        }
        if (other.gameObject.tag == "Enemy") {
            CameraController.currentCamera.setRetroShader(false);
            ParticleManager.Instance.initParticleSystem(ParticleManager.Instance.playerDeath, gameObject.transform.position);
            if (!isClone && isCloneActivated) {
                _clone.CloneBecomeMain();
            }
            AudioSource.PlayClipAtPoint(_deathSound, transform.position, GameManager.effectsVolume);
            currentLevel.lanes[currentPlane].setHighlight(false);
            GameManager.Instance.removeShip(this);
            other.gameObject.GetComponent<Enemy>().explode();
            Destroy(gameObject);
        }
    }

    //Activates rapid shot 
    public void ActivateRapid() {
        _rapidTime = RAPID_SHOT_TIME;
        isRapidActivated = true;
        if (!isClone && isCloneActivated) {
            _clone.ActivateRapid();
        }
    }

    public void SetRapidTime(float time){
        _rapidTime = time;
    }

    //Activates multi-shot
    public void ActivateMulti() {
        _multiTime = MULTI_SHOT_TIME;
        isMultiActivated = true;
        if (!isClone && isCloneActivated) {
            _clone.ActivateMulti();
        }
    }

    void SetMultiTime(float time)
    {
        _multiTime = time;
    }

    //Calls to spawn clone if no clone is currently activated and when clone power up is actvated 
    public void ActivateClone() {
        if (!isClone && !isCloneActivated) {
            SpawnClone();
        }
    }

    //When clone is activated, instantiate clone gameobject that mirrors player ship
    void SpawnClone() {
        isCloneActivated = true;
        GameObject playerClone = (GameObject)Instantiate(cloneObject);
        _clone = playerClone.GetComponent<Player>();
        _clone.initAsClone(currentLevel, currentPlane, positionOnPlane);
        if (isMultiActivated) {
            _clone.SetMultiTime(_multiTime);
            _clone.isMultiActivated = true;
        }
        if (isRapidActivated) {
            _clone.SetRapidTime(_rapidTime);
            _clone.isRapidActivated = true;
        }
    }

    //If clone is activated and main player ship dies, clone ship becomes main player ship
    public void CloneBecomeMain() {
        _isMovementMirrored = false;
        isClone = false;
        if (gameObject != null) {
            _escapeMenu = gameObject.GetComponent<EscapeMenu>();
        }
    }

#if UNITY_IPHONE
    void OnGUI() {
        GUIStyle highlightStyle = new GUIStyle(GUIManager.Instance.defaultStyle);
        highlightStyle.normal.textColor = Color.cyan;
        if (!_escapeMenu.currentlyActive && !isClone && !GameManager.Instance.enableSeebright) {
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(500)) / 2, 0,
                ScreenUtil.getPixelWidth(500), ScreenUtil.getPixelHeight(100)), "Menu", highlightStyle)) {
                _escapeMenu.display();
            }
        }
    }
#endif

}
