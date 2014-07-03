using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {
    public const string ID_PAWN = "pawn";
    public const string ID_CROSSHATCH = "crosshatch";
    public const string ID_SWIRLIE = "swirlie";
    public const string ID_CONFETTI = "confetti";


    public List<Edge> edges;
    public List<Lane> lanes;

    public bool wrapAround;
    public bool debugDraw = false;
    public GameObject cameraPosition;
    public int pawnCount = 0;
    public int crosshatchCount = 0;
    public int swirlieCount = 0;
    public int confettiCount = 0;

    private Lane _spawnLane;
    private List<string> _potentialEnemies;

    public List<string> PotentialEnemies {
        get {
            return _potentialEnemies;
        }
    }

    public Lane SpawnLane {
        get {
            return _spawnLane;
        }
        set {
            _spawnLane = value;
        }
    }

    void Start () {
        if (pawnCount < 0 || crosshatchCount < 0 || swirlieCount < 0 || confettiCount < 0) {
            Debug.LogError("Can't have a enemy spawn count lower than zero!");
        }
    }

    public void Shuffle(List<string> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = (int)(Random.value * (n + 1));
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void populatePotentialEnemies() {
        _potentialEnemies = new List<string>();
        for (int i = 0; i < pawnCount; i++) {
            _potentialEnemies.Add(ID_PAWN);
        }
        for (int i = 0; i < crosshatchCount; i++) {
            _potentialEnemies.Add(ID_CROSSHATCH);
        }
        for (int i = 0; i < swirlieCount; i++) {
            _potentialEnemies.Add(ID_SWIRLIE);
        }
        for (int i = 0; i < confettiCount; i++) {
            _potentialEnemies.Add(ID_CONFETTI);
        }

        int n = _potentialEnemies.Count;
        while (n > 1) {
            n--;
            int k = (int)(Random.value * (n + 1));
            string value = _potentialEnemies[k];
            _potentialEnemies[k] = _potentialEnemies[n];
            _potentialEnemies[n] = value;
        }
    }

    /**
     * Loads the level
     */
    public void load() {
        populatePotentialEnemies();
    }

    void Update() {

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
