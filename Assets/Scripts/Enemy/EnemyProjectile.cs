using UnityEngine;
using System.Collections;

public class EnemyProjectile : Enemy {
    //Time it takes to get to the end of a lane
    private const float DURATION = 1.5f;
    private float _startingTime;
    private Vector3 _extraVec;

    void Start () {
        _extraVec = new Vector3(1, 0, 0);
        _score = 10;
    }
	
    void Update () {
        if (!Alive) {
            return;
        }
        gameObject.transform.position = Vector3.Lerp(_currentLane.Back + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal,
            _currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec, (Time.time - _startingTime) / DURATION);
        if (gameObject.transform.position == _currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec) {
            Destroy(gameObject);
        }
    }

    public override void spawn(Lane spawnLane) {
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (gameObject.renderer.bounds.size.y/2) * _currentLane.Normal;
        _startingTime = Time.time;
    }
}
