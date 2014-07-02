using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public const float DELAY_NEXT_SHOT = .2f;
    public const float RAPID_SHOT_TIME = 15.0f;
    public const float MULTI_SHOT_TIME = 15.0f;

    public GameObject TestLevel;
    public Level currentLevel;

    public float mouseSensitivity;

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
    private bool _isClone = false; // is this ship a clone?

    public Player clone; // the player's clone
    public GameObject cloneObject; // the clone object

    // Use this for initialization
    void Start () {
        isRapidActivated = isMultiActivated = isCloneActivated = false;
        if (TestLevel != null) {
            currentLevel = TestLevel.GetComponent<Level>();
            init(currentLevel);
        }
        if (mouseSensitivity < .1f) {
            mouseSensitivity = .1f;
        }
        gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void init(Level level) {
        currentLevel = level;
        _alive = true;
    }

    public void initAsClone(Level level) {
        _isClone = true;
        currentLevel = level;
        _alive = true;
    }

    // Update is called once per frame
    void Update() {
        if (!_alive) {
            return;
        }
        float mouseMove = Input.GetAxis("Mouse X");
        float shipMove = mouseMove * mouseSensitivity;
        _positionOnPlane += shipMove;

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
            currentLevel.lanes[_currentPlane].setHighlight(false);
            GameManager.Instance.removeShip(this);
            Destroy(gameObject);
        }
    }

    public void ActivateRapid() {
        _rapidTime = RAPID_SHOT_TIME;
        isRapidActivated = true;
        if (!_isClone && isCloneActivated) {
            clone.ActivateRapid();
        }
    }

    public void ActivateMulti() {
        _multiTime = MULTI_SHOT_TIME;
        isMultiActivated = true;
        if (!_isClone && isCloneActivated) {
            clone.ActivateMulti();
        }
    }

    public void ActivateClone() {
        if (!_isClone && !isCloneActivated) {
            SpawnClone();
        }
    }

    void SpawnClone() {
        isCloneActivated = true;
        GameObject playerClone = (GameObject)Instantiate(cloneObject);
        clone = playerClone.GetComponent<Player>();
    }
}
