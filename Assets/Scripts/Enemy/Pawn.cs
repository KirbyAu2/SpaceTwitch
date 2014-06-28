using UnityEngine;
using System.Collections;

public class Pawn : Enemy {
    //Time it takes until next shot
    private const float SHOOTING_DELAY = 5.0f;

    private float _shootTime;

    void Start () {

    }
	
    void Update () {
        if (!Alive) {
            return;
        }
        if (Time.time > _shootTime) {
            _shootTime = Time.time + SHOOTING_DELAY;
            Debug.Log("Spawning Projectile!");
            spawnProjectile();
        }
    }

    public override void spawn(Lane spawnLane) {
        _shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back;
    }
}
