using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const float BASE_VELOCITY = 5.0f;

    public float speed = 1.0f;
    public Vector3 startingLocation;
    public Vector3 endingLocation;
    private Vector3 _extraVec;
    private Lane _currentLane;
    private float _startingTime = 0;

    void Start () {
        _extraVec = new Vector3(-1, 0, 0);
    }

    public void init(Lane currentLane) {
        _currentLane = currentLane;
        startingLocation = _currentLane.Front;
        endingLocation = _currentLane.Back;
        _startingTime = Time.time;
    }

    void Update () {

        gameObject.transform.position = Vector3.Lerp(_currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal,
            _currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec, (Time.time - _startingTime) / BASE_VELOCITY);
        if (gameObject.transform.position == _currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec) {
            Destroy(gameObject);
        }

        /*if (Vector3.Distance(startingLocation, transform.position) > Vector3.Distance(startingLocation, endingLocation)) {
            explode();
        }*/
        if(speed == 0) {
            return;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            Debug.Log("Collision!");
            PlayerProjectile p = collision.gameObject.GetComponent<PlayerProjectile>();
            //p.explode();
            Destroy(gameObject);
        }
    }

    /**
     * Makes the projectile explode
     */
    public void explode() {
        speed = 0;
        Destroy(gameObject);
    }
}
