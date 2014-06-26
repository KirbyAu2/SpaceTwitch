using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    public List<Level> levels;

    private int _score = 0;
    private int _multiplier = 0;
    private int _lives = 0;
    private int _currentLevel = 0;

    void Start () {
        if(_instance != null) {
            Debug.LogError("Can't initialize more than one instance of Game Manager!");
        }
        _instance = this;
        DontDestroyOnLoad(this);
        levels = new List<Level>();
    }

    public static GameManager Instance {
        get {
            return _instance;
        }
    }

    public Level CurrentLevel {
        get {
            return levels[_currentLevel];
        }
    }

    public int CurrentScore {
        get {
            return _score;
        }
    }

    public int Multiplier {
        get {
            return _multiplier;
        }
    }

    public int Lives {
        get {
            return _lives;
        }
    }

    void Update () {
        
    }
}
