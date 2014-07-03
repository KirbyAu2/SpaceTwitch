using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const float BASE_VELOCITY = .6f;

    public float speed = 1.0f;
    public Vector3 startingLocation;
    public Vector3 endingLocation;
    private Vector3 _extraVec;
    private Lane _currentLane;
    private float _startingTime = 0;

    public Player player; // the player object this shot came from

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
        float velocity = BASE_VELOCITY;
        if (player.isRapidActivated) {
            velocity /= 2;
        }
        gameObject.transform.position = Vector3.Lerp(_currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal,
            _currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec, (Time.time - _startingTime) / velocity);
        if (gameObject.transform.position == _currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec) {
            explode();
        }
        if(!renderer.enabled) {
            renderer.enabled = true;
        }

        if(speed == 0) {
            return;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (collision.gameObject == null)
            {
                return;
            }
            if (collision.gameObject.GetComponent<Spike>() != null)
            {
                if (collision.gameObject.GetComponent<Spike>().Invulernable)
                {
                    return;
                }
            }
            explode();
        }
    }

    /**
     * Makes the projectile explode
     */
    public void explode() {
        player.RemoveShot();
        speed = 0;
        Destroy(gameObject);
    }
}
