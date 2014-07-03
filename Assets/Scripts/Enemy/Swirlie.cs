using UnityEngine;
using System.Collections;

public class Swirlie : Enemy {
    private const float PATROL_TIME = 2.0f;
    private const float SHOOTING_DELAY = 3.0f;

    public GameObject spikePrefab;

    private float _startMovementTime;
    private float _shootTime;
    private Vector3 newPosition;
    private float _heightOffset;
    private Spike _headSpike;
    private Spike _tailSpike;

    public Spike TailSpike
    {
        get
        {
            return _tailSpike;
        }
        set
        {
            _tailSpike = value;
        }
    }

    void Start()
    {
        _score = 100;
        newPosition = transform.position;
        randomEnemyDrop();
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
        
        Vector3 positionA = _headSpike.gameObject.transform.position; //to do: make spikeHead
        Vector3 positionB = _tailSpike.gameObject.transform.position; //to do: make spikeTail
        if (transform.position == positionB)
        {
            newPosition = positionA;
        }
        else if (transform.position == positionA)
        {
            newPosition = positionB;
        }
        transform.position = Vector3.Lerp(positionA + _currentLane.Normal * _heightOffset, positionB + _currentLane.Normal * _heightOffset, .5f + .5f * Mathf.Sin(Time.time));
    }

    void OnDestroy()
    {
        _tailSpike.setVulnerability(true);
    }

    public override void spawn(Lane spawnLane) {
        _heightOffset = renderer.bounds.size.y / 2.0f;
        _startMovementTime = Time.time;
        _shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (renderer.bounds.size.y / 2) * _currentLane.Normal;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
        GameObject spike = (GameObject)Instantiate(spikePrefab);
        _headSpike = spike.GetComponent<Spike>();
        _headSpike.init(spawnLane, this);
    }
}
