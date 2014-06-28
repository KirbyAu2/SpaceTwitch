using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public Level currentLevel;

    public float mouseSensitivity;

    private int _currentPlane;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)

    public GameObject playerProjectile;

    // Use this for initialization
    void Start () {
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
        else if (_currentPlane > currentLevel.lanes.Count) {
            if (currentLevel.wrapAround) {
                _currentPlane -= currentLevel.lanes.Count;
            }
            else {
                _currentPlane = currentLevel.lanes.Count - 1;
                _positionOnPlane = 1;
            }
        }

        // update position
        transform.position = currentLevel.lanes[_currentPlane].Front;
        transform.up = currentLevel.lanes[_currentPlane].Normal;

        // shoot
        if (Input.GetMouseButton(0)) {
            Shoot();
        }
    }

    void Shoot() {
        GameObject shot = (GameObject)Instantiate(playerProjectile);
        Vector3 start = shot.GetComponent<PlayerProjectile>().startingLocation = currentLevel.lanes[_currentPlane].Front;
        Vector3 end = shot.GetComponent<PlayerProjectile>().endingLocation = currentLevel.lanes[_currentPlane].Back;
        shot.rigidbody.velocity = Vector3.Normalize(end - start) * PlayerProjectile.BASE_VELOCITY;
    }
}
