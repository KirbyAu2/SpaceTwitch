﻿using UnityEngine;
using System.Collections;

public class EnemyProjectile : Enemy {
    //Time it takes to get to the end of a lane
    private const float BASE_VELOCITY = 20.0f;
    private const float DURATION = .4f;
    public Vector3 startLocation;
    private Vector3 _extraVec;
    private float _velocity;

    void Start () {
        _extraVec = new Vector3(1, 0, 0);
        _score = 10;
        startLocation = Vector3.zero;
    }
	
    void Update () {
        if (!Alive) {
            return;
        }
        gameObject.rigidbody.velocity = new Vector3(_velocity, 0, 0);
        if (gameObject.transform.position.x >= (_currentLane.Front + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal + _extraVec).x) {
            Destroy(gameObject);
        }
        if (!renderer.enabled) {
            renderer.enabled = true;
        }
    }

    public override void spawn(Lane spawnLane) {
        if (startLocation == Vector3.zero) {
            startLocation = spawnLane.Back;
        }
        _currentLane = spawnLane;
        _alive = true;
        _velocity = BASE_VELOCITY;
        gameObject.transform.position = startLocation + (gameObject.renderer.bounds.size.y / 2) * _currentLane.Normal;
    }
}
