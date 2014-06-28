using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public List<Edge> edges;
    public bool wrapAround;
    
    private List<Enemy> _potentialEnemies;

    public List<Enemy> PotentialEnemies {
        get {
            return _potentialEnemies;
        }
    }

    void Start () {
        _potentialEnemies = new List<Enemy>();
    }

    void Update () {

    }
}
