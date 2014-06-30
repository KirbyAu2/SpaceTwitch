using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
    protected bool _alive = false;
    protected Lane _currentLane;
    protected int _score = 0;

    public bool Alive {
        get {
            return _alive;
        }
    }

    public Lane CurrentLane {
        get {
            return _currentLane;
        }
    }

    void Start () {
        
    }

    void Update () {

    }

    protected void spawnProjectile() {
        GameObject p = (GameObject)Instantiate(EnemyManager.Instance.enemyProjectilePrefab);
        p.GetComponent<EnemyProjectile>().spawn(_currentLane);
    }

    /**
     * Implement this function. This will be called when an enemy spawns!
     */
    abstract public void spawn(Lane spawnLane);

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision with player?!!");
        if(collision.gameObject.tag == "PlayerProjectile") {
            PlayerProjectile p = collision.gameObject.GetComponent<PlayerProjectile>();
            _alive = false;
            EnemyManager.Instance.removeEnemy(this);
            p.explode();
            Destroy(gameObject);
            Score.CurrentScore += _score;
        }
    }
}
