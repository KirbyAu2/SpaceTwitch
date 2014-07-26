using UnityEngine;
using System.Collections;

/*
 * The Swirlie enemy will move back and forth on the Spikes enemy
 * Swirlie will shoot projectiles down the lane 
 * Swirlie must be killed for the player to damage the Spikes enemy
 */
public class Swirlie : Enemy {
    private const float PATROL_SPEED = 3.0f;
    private const float SHOOTING_DELAY = 3.0f;

    public GameObject spikePrefab;

    private float _shootTime;
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

    /*
     * Calls randomEnemyDrop() when spawned to see if the enemy will drop powerup 
     */
    void Start()
    {
        _score = 200;
        randomEnemyDrop();
    }

    /*
     * Swirlie shoots projectile
     * Calls PositionChanging() to make it move
     */ 
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

    /*
     * Moves the Swirlie up and down the growing spike
     */
    void PositionChanging() {
        Vector3 positionA = _headSpike.gameObject.transform.position;
        Vector3 positionB = _tailSpike.gameObject.transform.position;
        transform.position = Vector3.Lerp(positionA + _currentLane.Normal * _heightOffset, positionB + _currentLane.Normal * _heightOffset, 
            .5f + .5f * Mathf.Sin(_currentLane.Front.z + Time.time * PATROL_SPEED));
    }

    /*
     * When Swirlie is destroyed, the Spike is now vulnerable
     */
    void OnDestroy()
    {
        _tailSpike.setVulnerability(true);
    }

    /*
     * Spawns Swirlie on same lane as spike
     */
    public override void spawn(Lane spawnLane) {
        _heightOffset = renderer.bounds.size.y / 2.0f;
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
