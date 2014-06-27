using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
    private bool _alive = true;

    public bool Alive {
        get {
            return _alive;
        }
    }

    void Start () {
        
    }

    void Update () {

    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "PlayerProjectile") {
            PlayerProjectile p = collision.gameObject.GetComponent<PlayerProjectile>();
            p.explode();
        }
    }
}
