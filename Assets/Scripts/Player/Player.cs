using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public const float DELAY_NEXT_SHOT = .2f;

    public GameObject TestLevel;
    public Level currentLevel;

    public float mouseSensitivity;
    private float _nextShotCooldown;

    private int _currentPlane = 0;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)

    private bool _alive = false;

    public GameObject playerProjectile;

    // Use this for initialization
    void Start () {
        if (TestLevel != null) {
            currentLevel = TestLevel.GetComponent<Level>();
            init(currentLevel);
        }
        if (mouseSensitivity < .1f) {
            mouseSensitivity = .1f;
        }
        _nextShotCooldown = Time.time;
        gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void init(Level level) {
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

        //print("Plane: " + _currentPlane + ", Position: " + _positionOnPlane);

        // update position
        transform.position = currentLevel.lanes[_currentPlane].Front;
        transform.up = -currentLevel.lanes[_currentPlane].Normal;

        // shoot
        if (Input.GetMouseButton(0) && _nextShotCooldown < Time.time) {
            Shoot();
        }
    }

    public Lane CurrentLane {
        get {
            return currentLevel.lanes[_currentPlane];
        }
    }

    void Shoot() {
        _nextShotCooldown = Time.time + DELAY_NEXT_SHOT;
        GameObject shot = (GameObject)Instantiate(playerProjectile);
        Lane currentLane = currentLevel.lanes[_currentPlane];
        Vector3 start = currentLane.Front + ((gameObject.renderer.bounds.size.y / 2) * currentLane.Normal);
        shot.GetComponent<PlayerProjectile>().init(currentLevel.lanes[_currentPlane]);
        shot.transform.position = start;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy") {
            GameManager.Instance.removeShip(this);
            Destroy(gameObject);
        }
    }
}
