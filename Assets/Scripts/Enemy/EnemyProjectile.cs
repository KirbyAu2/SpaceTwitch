using UnityEngine;
using System.Collections;

public class EnemyProjectile : Enemy {
    //Time it takes to get to the end of a lane
    private const float DURATION = .75f;
    private float _startingTime;
    public Vector3 startLocation;
    private Vector3 _extraVec;

    void Start () {
        _extraVec = new Vector3(1, 0, 0);
        _score = 10;
    }
	
    void Update () {
        if (!Alive) {
            return;
        }
        gameObject.transform.position = Vector3.Lerp(startLocation,
            _currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec, (Time.time - _startingTime) / DURATION);
        if (gameObject.transform.position == _currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec) {
            Destroy(gameObject);
        }
    }

    public override void spawn(Lane spawnLane) {
        startLocation = spawnLane.Back;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (gameObject.renderer.bounds.size.y/2) * _currentLane.Normal;
        _startingTime = Time.time;
    }
}
