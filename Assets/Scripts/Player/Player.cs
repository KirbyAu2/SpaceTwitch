using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public GameObject TestLevel;
    public Level currentLevel;

    public float mouseSensitivity;

    private int _currentPlane = 0;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)

    public int maxShots = 5;
    public float reloadTime = .2f;
    private float _reload = 0;

    public GameObject playerProjectile;

    // Use this for initialization
    void Start () {
        currentLevel = TestLevel.GetComponent<Level>();
        if (mouseSensitivity < .1f) {
            mouseSensitivity = .1f;
        }
    }

    // Update is called once per frame
    void Update() {
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
        float angleUp = Vector3.Angle(Vector3.up, currentLevel.lanes[_currentPlane].Normal) - 90;
        float angleRight = Vector3.Angle(Vector3.forward, currentLevel.lanes[_currentPlane].Normal);
        float angleLeft = Vector3.Angle(Vector3.back, currentLevel.lanes[_currentPlane].Normal);
        if (angleRight < angleLeft) {
            angleUp = -angleUp;
        }
        transform.eulerAngles = new Vector3(angleUp, 180, 0);

        // shoot
        if (Input.GetMouseButton(0)) {
            if (_reload < 0) {
                Shoot();
                _reload = reloadTime;
            }
        }

    }

    void Shoot() {
        GameObject shot = (GameObject)Instantiate(playerProjectile);
        Lane currentLane = currentLevel.lanes[_currentPlane];
        Vector3 start = currentLane.Front + ((gameObject.renderer.bounds.size.y / 2) * currentLane.Normal);
        Vector3 end = currentLane.Back + ((gameObject.renderer.bounds.size.y / 2) * currentLane.Normal);
        shot.GetComponent<PlayerProjectile>().startingLocation = start;
        shot.GetComponent<PlayerProjectile>().endingLocation = end;
        shot.rigidbody.velocity = Vector3.Normalize(end - start) * shot.GetComponent<PlayerProjectile>().speed;
        shot.transform.position = start;
    }
}
