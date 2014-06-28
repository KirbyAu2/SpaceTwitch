using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public List<Edge> edges;
    public List<Lane> lanes;
    public bool wrapAround;
    public bool debugDraw = false;

    int pawnCount = 0;
    int crosshatchCount = 0;
    int swirlieCount = 0;
    int confettiCount = 0;
    
    private List<Enemy> _potentialEnemies;

    public List<Enemy> PotentialEnemies {
        get {
            return _potentialEnemies;
        }
    }

    void Start () {
        _potentialEnemies = new List<Enemy>();
        GameManager.Instance.debugLevel(this);
    }

    void Update () {

    }

    public Lane getRandomLane() {
        return lanes[(int)(Random.value * lanes.Count)];
    }

    void OnDrawGizmos() {
        if (!debugDraw) {
            return;
        }
        if (edges == null || lanes == null) {
            return;
        }

        bool flip = true;
        foreach (Lane l in lanes) {
            Gizmos.color = (flip)?Color.green:Color.blue;
            flip = !flip;

            Gizmos.DrawLine(l.LeftEdge.Front, l.RightEdge.Front);
            Gizmos.DrawLine(l.RightEdge.Front, l.RightEdge.Back);
            Gizmos.DrawLine(l.RightEdge.Back, l.LeftEdge.Back);
            Gizmos.DrawLine(l.LeftEdge.Back, l.LeftEdge.Front);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(l.Front, 0.2f);
            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(l.Back, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(l.Front, l.Front + l.Normal);
        }
    }
}
