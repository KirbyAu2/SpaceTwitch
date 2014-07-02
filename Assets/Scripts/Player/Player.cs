using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public const float DELAY_NEXT_SHOT = .2f;
    public const float RAPID_SHOT_TIME = 15.0f;
    public const float MULTI_SHOT_TIME = 15.0f;

<<<<<<< HEAD
    private const float CAMERA_PERCENT_BACK = .5f;
=======
    private float _mouseSensitivity;
>>>>>>> origin/master

    public GameObject TestLevel;
    public Level currentLevel;

    private int _currentPlane = 0;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)
    
    private bool _alive = false;
    public int maxShots = 5;
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

    // Use this for initialization
    void Start () {
        isRapidActivated = isMultiActivated = isCloneActivated = false;
        if (TestLevel != null) {
            currentLevel = TestLevel.GetComponent<Level>();
            init(currentLevel);
        }

        _mouseSensitivity = GameManager.Instance.mouseSensitivity;
        if (_mouseSensitivity < .1f) {
            _mouseSensitivity = .1f;
        }
        GameManager.Instance.addShip(this);
        gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void init(Level level) {
        currentLevel = level;
        _alive = true;
    }

    public void initAsClone(Level level, int plane, float position) {
        isClone = true;
        currentLevel = level;
        _alive = true;
        // initialize clone location
        if (!currentLevel.wrapAround) { // level doesn't wrap
            _isMovementMirrored = true;
            _currentPlane = level.lanes.Count - (plane + 1);
            _positionOnPlane = 1 - position;
        }
        else {
            _currentPlane = plane - (level.lanes.Count / 2);
            if (_currentPlane < 0) {
                _currentPlane += level.lanes.Count;
            }
            _positionOnPlane = position;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) { // testing purposes
            ActivateClone();
            ActivateMulti();
            ActivateRapid();
        }

        if (!_alive) {
            return;
        }
        float mouseMove = Input.GetAxis("Mouse X");
        float shipMove = mouseMove * _mouseSensitivity;
        if (_isMovementMirrored) {
            _positionOnPlane -= shipMove;
        }
        else {
            _positionOnPlane += shipMove;
        }

        currentLevel.lanes[_currentPlane].setHighlight(false);
        // calculate new position after movement
        if (_positionOnPlane < 0) {
            _currentPlane--;
            _positionOnPlane++;
        }
        else if (_positionOnPlane > 1) {
            _currentPlane++;
            _positionOnPlane--;
        }

        if (_currentPlane < 0) {
            if (currentLevel.wrapAround) {
                _currentPlane += currentLevel.lanes.Count;
            }
            else {
                _currentPlane = 0;
                _positionOnPlane = 0;
            }
        }
        else if (_currentPlane >= currentLevel.lanes.Count) {
            if (currentLevel.wrapAround) {
                _currentPlane -= currentLevel.lanes.Count;
            }
            else {
                _currentPlane = currentLevel.lanes.Count - 1;
                _positionOnPlane = 1;
            }
        }
        currentLevel.lanes[_currentPlane].setHighlight(true);
        //print("Plane: " + _currentPlane + ", Position: " + _positionOnPlane);

        // update position
        transform.position = currentLevel.lanes[_currentPlane].Front;
        float angleUp = Vector3.Angle(Vector3.up, currentLevel.lanes[_currentPlane].Normal) - 90;
        float angleRight = Vector3.Angle(Vector3.forward, currentLevel.lanes[_currentPlane].Normal);
        float angleLeft = Vector3.Angle(Vector3.back, currentLevel.lanes[_currentPlane].Normal);
        if (angleRight < angleLeft) {
            angleUp = -angleUp;
        }
        transform.eulerAngles = new Vector3(angleUp, 180, 0);

        // shoot
        int maxMultiShots = maxShots;
        if (isMultiActivated) {
            maxMultiShots *= 3;
        }
        if (Input.GetMouseButton(0) && _reload < 0 && _numShots < maxMultiShots) {
            Shoot();
            _reload = DELAY_NEXT_SHOT;
        
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

        setCamera();
        if (_clone == null) {
            isCloneActivated = false;
        }
    }

    private void setCamera() {
        //Vector3 cameraPos = CameraController.currentCamera.gameObject.transform.position;
        CameraController.currentCamera.gameObject.transform.position =
            new Vector3(gameObject.transform.position.x + currentLevel.gameObject.renderer.bounds.size.z * CAMERA_PERCENT_BACK,
                        currentLevel.gameObject.renderer.bounds.size.y/1.6f,
                        0
                        );

    }
    
    public Lane CurrentLane {
        get {
            if(currentLevel != null && currentLevel.lanes != null) {
                return currentLevel.lanes[_currentPlane];
            } else {
                return null;
            }
        }
    }

    void Shoot() {
        Lane currentLane = currentLevel.lanes[_currentPlane];
        GameObject shot = (GameObject)Instantiate(playerProjectile);
        shot.renderer.enabled = false;
        shot.GetComponent<PlayerProjectile>().player = this;
        shot.GetComponent<PlayerProjectile>().init(currentLane);
        _numShots++;
        if (isMultiActivated) {
            currentLane = currentLevel.lanes[_currentPlane].LeftLane;
            if (currentLane != null) {
                shot = (GameObject)Instantiate(playerProjectile);
                shot.GetComponent<PlayerProjectile>().player = this;
                shot.GetComponent<PlayerProjectile>().init(currentLane);
                _numShots++;
            }
            currentLane = currentLevel.lanes[_currentPlane].RightLane;
            if (currentLane != null) {
                shot = (GameObject)Instantiate(playerProjectile);
                shot.GetComponent<PlayerProjectile>().player = this;
                shot.GetComponent<PlayerProjectile>().init(currentLane);
                _numShots++;
            }
        }
    }
    
    public void RemoveShot() {
        _numShots--;
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy") {
            if (!isClone && isCloneActivated) {
                _clone.CloneBecomeMain();
            }
            currentLevel.lanes[_currentPlane].setHighlight(false);
            GameManager.Instance.removeShip(this);
            Destroy(gameObject);
        }
    }

    public void ActivateRapid() {
        _rapidTime = RAPID_SHOT_TIME;
        isRapidActivated = true;
        if (!isClone && isCloneActivated) {
            _clone.ActivateRapid();
        }
    }

    public void ActivateMulti() {
        _multiTime = MULTI_SHOT_TIME;
        isMultiActivated = true;
        if (!isClone && isCloneActivated) {
            _clone.ActivateMulti();
        }
    }

    public void ActivateClone() {
        if (!isClone && !isCloneActivated) {
            SpawnClone();
        }
    }

    void SpawnClone() {
        isCloneActivated = true;
        GameObject playerClone = (GameObject)Instantiate(cloneObject);
        _clone = playerClone.GetComponent<Player>();
        _clone.initAsClone(currentLevel, _currentPlane, _positionOnPlane);
    }

    public void CloneBecomeMain() {
        _isMovementMirrored = false;
        isClone = false;
    }

}
