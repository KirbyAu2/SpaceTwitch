using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const int BASE_VELOCITY = 5;

    public float speed = 1f;
    public Vector3 startingLocation;
    public Vector3 endingLocation;

    void Start () {

    }

    void Update () {
        if (Vector3.Distance(startingLocation, transform.position) > Vector3.Distance(startingLocation, endingLocation)) {
            explode();
        }
        if(speed == 0) {
            return;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            PlayerProjectile p = collision.gameObject.GetComponent<PlayerProjectile>();
            p.explode();
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
