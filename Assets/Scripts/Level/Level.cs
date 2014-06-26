using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public List<Vector3> edges;
    public bool wrapAround;
    
    private List<Enemy> _potentialEnemies;

    public List<Enemy> PotentialEnemies {
        get {
            return _potentialEnemies;
        }
    }

    void Start () {
        
    }

    void Update () {

    }
}
