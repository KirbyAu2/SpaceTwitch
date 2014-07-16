using UnityEngine;
using System.Collections;

/*
 * The Pawn enemy is the most basic enemy that spawns on a random lane
 * Pawns shoot projectiles down the lane that can damage the player
 */
public class Pawn : Enemy {
    //Time it takes until next shot
    private const float SHOOTING_DELAY = 3.0f;

    private float _shootTime;

    /*
     * calls randomEnemyDrop() when spawned
     */
    void Start () {
        _score = 100;
        randomEnemyDrop();
    }
	
    /*
     * Pawn shoots projectiles
     */
    void Update () {
        if (!Alive) {
            return;
        }
        if (Time.time > _shootTime) {
            _shootTime = Time.time + SHOOTING_DELAY;
            spawnProjectile();
        }
    }

    /*
     * Initializes and spawns pawn
     */
    public override void spawn(Lane spawnLane) {
        _shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (renderer.bounds.size.y/2) * _currentLane.Normal;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}
