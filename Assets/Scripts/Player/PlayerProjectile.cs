using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const int BASE_VELOCITY = 5;

    public float damage { get; private set; }

    public int speed = 1;
    public Vector3 startingLocation;
    public Vector3 endingLocation;

    void Start () {

    }

    void Update () {
        if (Vector3.Distance(startingLocation, transform.position) > Vector3.Distance(startingLocation, endingLocation)) {
            Explode();
        }
        if(speed == 0) {
            return;
        }
    }

    /**
     * Makes the projectile explode
     */
    public void Explode() {
        speed = 0;
        Destroy(gameObject);
    }
}
