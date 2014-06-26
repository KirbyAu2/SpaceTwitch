using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public const int BASE_VELOCITY = 5;

    public int speed = 1;
    public Vector3 startingLocation;
    public Vector3 endingLocation;

    void Start () {

    }

    void Update () {
        if(speed == 0) {
            return;
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
