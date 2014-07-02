using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy
{
    public const float DEFAULT_DIST_FROM_BACK = .2f;
    public const int MAX_SPIKE_COUNT = 5;
    private const float SPAWN_NEXT_SPIKE_DURATION = 3.0f;

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

    public void setVulnerability(bool value)
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

    private void AddSpike()
    {
        GameObject c = (GameObject)Instantiate(spikePrefab);
        _child = c.GetComponent<Spike>();
        _child.init(_currentLane,_swirlie,this);
        _head.NumOfSpikes++;
    }

    private void RemoveSpike(Spike s)
    {
        if (s == _child)
        {
            _child = null;
        }
        _swirlie.TailSpike = this;
        _head.NumOfSpikes--;
        _startTime = Time.time;
    }

    void Start()
    {

    }

    void Update()
    {
        if (_head.NumOfSpikes > MAX_SPIKE_COUNT - 2 || _child != null) {
            return;
        }
        if (Time.time > _startTime + SPAWN_NEXT_SPIKE_DURATION)
        {
            AddSpike();
        }
    }

    void OnDestroy() {
        if (this != _head)
        {
            _parent.setVulnerability(!_invulnerable);
            _parent.RemoveSpike(this);
        }

    }

    public override void spawn(Lane spawnLane)
    {
        throw new System.NotImplementedException();
    }

    public void init(Lane spawnLane, Swirlie swirlie, Spike parent = null) {
        _swirlie = swirlie;
        _parent = parent;
        _head = getHead();
        _invulnerable = (parent != null)?_parent.Invulernable:true;
        if (_parent == null)
        {
            gameObject.transform.position = (spawnLane.Front - spawnLane.Back) * DEFAULT_DIST_FROM_BACK + spawnLane.Back;
        }
        else
        {
            Debug.Log((spawnLane.Front - _parent.gameObject.transform.position));
            gameObject.transform.position = (spawnLane.Front - _parent.gameObject.transform.position).normalized * spikePrefab.renderer.bounds.size.x + _parent.gameObject.transform.position;
        }
        swirlie.TailSpike = this;
        _startTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}