using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy
{
    public float duration = 0.5f;
    public float starttime;
    public float currenttime;
    public int numOfSpikes = 0;
    public Spike parent;
    private List<Spike> _spike;
    public Spike headSpike;
    public Spike newSpike;
    public Spike tailSpike;

    public Spike getHead()
    {
        Spike current = this;
        while (current.parent != null)
        {
            current = current.parent;
        }
        headSpike = current;
        return current;
    }

    private void AddSpike()
    {
        if (headSpike == null){
            headSpike = new Spike();
        }
        else{
            headSpike.AddSpike();
        }
        numOfSpikes++;
    }

    private void RemoveSpike()
    {
        /*if () {  //to do: if tailSpike==headSpike
            //remove headSpike
            //_alive = false     Spike is dead
        }
        else {
            //remove tailSpike
            //tailSpike = tailSpike.parent
            // count--; 
        }*/
    }

    void Start()
    {
        //list = new MyList();
        //Spike _spike = new List<Spike>();

    }

    void Update()
    {
        /*
        if (numOfSpikes <= 5)
        {
            if (Time.time >= starttime + duration)
            {
                AddSpike();
            }
        }
        if () { //To do: if swirlie is dead
            if (){ //To do: if spike is hit
                RemoveSpike();
            }
        }*/
    }

    public override void spawn(Lane spawnLane)
    {
        //_shootTime = Time.time;
        _currentLane = spawnLane;
        _alive = true;
        gameObject.transform.position = _currentLane.Back + (renderer.bounds.size.y / 2) * _currentLane.Normal;
        transform.rotation = Quaternion.LookRotation(_currentLane.Normal);
    }
}
 /* Questions:
        How do I say "if swirlie is dead" ?
        How did you check collision for player projectile and spike?
        */