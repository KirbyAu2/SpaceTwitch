using UnityEngine;
using System.Collections;

public class Pawn : Enemy {
    //Time it takes until next shot
    private const float SHOOTING_DELAY = 3.0f;

    private float _shootTime;

    void Start () {
        _score = 100;
        randomEnemyDrop();
    }
	
    void Update () {
        if (!Alive) {
            return;
        }
        if (Time.time > _shootTime) {
            _shootTime = Time.time + SHOOTING_DELAY;
            spawnProjectile();
        }
    }

    public override void spawn(Lane spawnLane) {
        _shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (renderer.bounds.size.y/2) * _currentLane.Normal;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}
