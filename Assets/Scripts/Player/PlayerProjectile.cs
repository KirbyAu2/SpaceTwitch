using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const float BASE_VELOCITY = 10.0f;

    private Vector3 _extraVec;
    private Lane _currentLane;
    private float _velocity;

    public Player _player; // the player object this shot came from

    void Start () {
        _extraVec = new Vector3(-10, 0, 0);
    }

    public void init(Lane currentLane, Player p) {
        _player = p;
        _currentLane = currentLane;
        gameObject.transform.position = _currentLane.Front + gameObject.renderer.bounds.size.y/2 * _currentLane.Normal;
        _velocity = BASE_VELOCITY;
        if (_player.isRapidActivated) {
            _velocity *= 2.0f;
        }
    }

    void Update () {
        gameObject.rigidbody.velocity = new Vector3(-_velocity, 0, 0);
        if (gameObject.transform.position.x <= (_currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec).x) {
            explode();
        }
        if(!renderer.enabled) {
            renderer.enabled = true;
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
        _player.RemoveShot();
        Destroy(gameObject);
    }
}
