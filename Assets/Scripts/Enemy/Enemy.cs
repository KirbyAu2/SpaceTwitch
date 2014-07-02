using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
    protected bool _alive = false;
    protected Lane _currentLane;
    protected int _score = 0;
    protected bool _invulnerable = false;

    public bool Alive {
        get {
            return _alive;
        }
    }

    public bool Invulernable
    {
        get
        {
            return _invulnerable;
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

    public void setVulnerability(bool value) {
        _invulnerable = !value;
    }

    protected void spawnProjectile() {
        GameObject p = (GameObject)Instantiate(EnemyManager.Instance.enemyProjectilePrefab);
        p.GetComponent<EnemyProjectile>().spawn(_currentLane);
        p.GetComponent<EnemyProjectile>().startLocation = gameObject.transform.position;
    }

    /**
     * Implement this function. This will be called when an enemy spawns!
     */
    abstract public void spawn(Lane spawnLane);

    void OnCollisionEnter(Collision collision) {
        if (_invulnerable)
        {
            return;
        }
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
