using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The Level class manages Levels and enemy count
 * Each level has a different count of enemies depending on difficulty
 */
public class Level : MonoBehaviour {
    public const string ID_PAWN = "pawn";
    public const string ID_CROSSHATCH = "crosshatch";
    public const string ID_SWIRLIE = "swirlie";
    public const string ID_CONFETTI = "confetti";


    public List<Edge> edges;
    public List<Lane> lanes;
    public List<Spike> spikeList;

    public bool wrapAround;
    public bool debugDraw = false;
    public GameObject cameraPosition;
    public int pawnCount = 0;
    public int crosshatchCount = 0;
    public int swirlieCount = 0;
    public int confettiCount = 0;
    public bool isTutorial = false;

    private Tutorial _tutorial;
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
        if (isTutorial) {
            _tutorial = gameObject.AddComponent<Tutorial>();
        }

        //Algorithm to get number of enemies in each level depending on difficulty
        pawnCount *= (GameManager.Instance.CurrentDifficulty/5 + 1);
        crosshatchCount *= (GameManager.Instance.CurrentDifficulty/5 + 1);
        swirlieCount *= (GameManager.Instance.CurrentDifficulty/5 + 1);
        confettiCount *= (GameManager.Instance.CurrentDifficulty/5 + 1);
    }

    public Tutorial Tutorial {
        get {
            return _tutorial;
        }
    }

    public int getLaneIndex(Lane l) {
        return lanes.IndexOf(l);
    }

    /*
     * Adds enemies to list of potential enemies 
     * Uses for loop to add all enemies according to count
     */
    private void populatePotentialEnemies() {
        _potentialEnemies = new List<string>();
        for (int i = 0; i < pawnCount; i++) {
            _potentialEnemies.Add(ID_PAWN);
        }
        for (int i = 0; i < swirlieCount; i++) {
            _potentialEnemies.Add(ID_SWIRLIE);
        }
        for (int i = 0; i < confettiCount; i++) {
            _potentialEnemies.Add(ID_CONFETTI);
        }
        for (int i = 0; i < crosshatchCount; i++) {
            _potentialEnemies.Add(ID_CROSSHATCH);
        }

        if (isTutorial) {
            return;
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

    //When player beats level and level is destroyed, destroy all spike gameObjects in spikeList
    void OnDestroy()
    {
        foreach(Spike spike in spikeList){
            if (spike != null) {
                if (spike.gameObject != null) {
                    Destroy(spike.gameObject);
                }
            }
        }
    }

    //gets random lane
    public Lane getRandomLane() {
        return lanes[(int)(Random.value * lanes.Count)];
    }

    /*
     * Draws gizmos that are also pickable and always drawn. 
     * Allows us to quickly pick important objects in scene 
     * Helps with debugging
     */
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
