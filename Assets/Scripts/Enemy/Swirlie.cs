using UnityEngine;
using System.Collections;

public class Swirlie : Enemy {

    //Time it takes until next shot
    private const float SHOOTING_DELAY = 3.0f;

    private float _shootTime;
    private Vector3 newPosition;
    void Start()
    {
        _score = 100;
        newPosition = transform.position;
    }

    void Update() {
        if (!Alive)
        {
            return;
        }
        if (Time.time > _shootTime)
        {
            _shootTime = Time.time + SHOOTING_DELAY;
            spawnProjectile();
        }
        PositionChanging();
    }

    void PositionChanging() {
        
        Vector3 positionA = _currentLane.Front;//headSpike.gameObject.transform.position; //to do: make spikeHead
        Vector3 positionB = _currentLane.Back; //tailSpike.gameObject.transform.position; //to do: make spikeTail
        if (transform.position == positionB)
        {
            newPosition = positionA;
        }
        else if (transform.position == positionA)
        {
            newPosition = positionB;
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
    }


    public override void spawn(Lane spawnLane)
    {
        _shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (renderer.bounds.size.y / 2) * _currentLane.Normal;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}
