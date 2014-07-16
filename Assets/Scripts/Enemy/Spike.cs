using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy
{
    public const float DEFAULT_DIST_FROM_BACK = .2f;
    public const int MAX_SPIKE_COUNT = 5;
    private const float SPAWN_NEXT_SPIKE_DURATION = 1.0f;

    public GameObject spikePrefab;

    private float _startTime;
    private Spike _parent;
    private Spike _child;
    private Spike _head;
    private int _numOfSpikes = 0;
    private Swirlie _swirlie;

    public Spike getHead() {
        Spike current = this;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current;
    }

    public new void setVulnerability(bool value)
    {
        base.setVulnerability(value);
        if (_child)
        {
            _child.setVulnerability(value);
        }
    }

    public int NumOfSpikes
    {
        get
        {
            return _numOfSpikes;
        }
        set
        {
            _numOfSpikes = value;
        }
    }

    public Spike Parent {
        get {
            return _parent;
        }
    }

    /*
     * Adds Spike
     */
    private void AddSpike()
    {
        GameObject c = (GameObject)Instantiate(spikePrefab);
        _child = c.GetComponent<Spike>();
        _child.init(_currentLane,_swirlie,this);
        _head.NumOfSpikes++;
    }

    /*
     * Removes Spike
     */
    private void RemoveSpike(Spike s)
    {
        if (s == _child)
        {
            _child = null;
            _invulnerable = false;
        }
        _swirlie.TailSpike = this;
        _head.NumOfSpikes--;
        _startTime = Time.time;
    }

    void Start()
    {
        _score = 50;
        GameManager.Instance.CurrentLevel.spikeList.Add(this);
    }

    /*
     * Toggles invulnerability depending on whether swirlie is still alive
     * Checks for max spike count
     * Adds spikes after time duration 
     */
    void Update()
    {
        if (_swirlie == null) {
            _invulnerable = false;
        } else if (_swirlie.gameObject == null) {
            _swirlie = null;
            _invulnerable = false;
        }
        gameObject.collider.enabled = !_invulnerable;
        if (_head.NumOfSpikes > MAX_SPIKE_COUNT - 2 || _child != null) {
            return;
        }
        if (Time.time > _startTime + SPAWN_NEXT_SPIKE_DURATION)
        {
            AddSpike();
        }
    }

    /*
     * When Spike is hit, destroys last spike and sets vulnerability to parent spike
     */
    void OnDestroy() {
        if (this != _head)
        {
            _parent.setVulnerability(true);
            _parent.RemoveSpike(this);
            if (_child != null) {
                Destroy(_child.gameObject);
            }
        }

    }

    public override void spawn(Lane spawnLane)
    {
        throw new System.NotImplementedException();
    }
    /*
     * Initializes position to spawn new spike
     */
    public void init(Lane spawnLane, Swirlie swirlie, Spike parent = null) {
        _swirlie = swirlie;
        _parent = parent;
        _head = getHead();
        _invulnerable = swirlie != null;
        if (_parent == null)
        {
            gameObject.transform.position = (spawnLane.Front - spawnLane.Back) * DEFAULT_DIST_FROM_BACK + spawnLane.Back;
        }
        else
        {
            gameObject.transform.position = (spawnLane.Front - _parent.gameObject.transform.position).normalized * spikePrefab.renderer.bounds.size.x + _parent.gameObject.transform.position;
        }
        swirlie.TailSpike = this;
        _startTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}