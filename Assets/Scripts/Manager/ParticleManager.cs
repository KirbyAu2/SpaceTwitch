using UnityEngine;
using System.Collections.Generic;

/*
 * ParticleManager class manages particle systems 
 * Particle effects occur when player ship or enemies are destroyed, and when levels are transistioned.
 * Particle effects are also used to show to player which enemies will drop power-ups
 */
public class ParticleManager : MonoBehaviour {
    private static ParticleManager _instance;

    public GameObject playerDeath;
    public GameObject enemyDeath;

    private List<ParticleSystem> _items;
    private Dictionary<GameObject, float> _startTimes;

    void Start () {
        //Implements Singleton
	    if(_instance != null) {
            Debug.LogError("Can't have more than one instance of ParticleManager");
            return;
        }
        _startTimes = new Dictionary<GameObject, float>();
        _items = new List<ParticleSystem>();
        _instance = this;
    }

    public static ParticleManager Instance {
        get {
            return _instance;
        }
    }
    /*
     * Initializes particle system 
     * Instantiates game object
     */
    public void initParticleSystem(GameObject pSystem, Vector3 pos) {
        GameObject g = (GameObject)Instantiate(pSystem);
        ParticleSystem sys = g.GetComponent<ParticleSystem>();
        if (sys == null) {
            Destroy(g);
            Debug.LogError("Game Object has no particle system!");
            return;
        }
        g.gameObject.transform.position = pos;
        _items.Add(sys);
        _startTimes[g] = Time.time;
    }
	
    /*
     * Destroys gameobject after a duration
     */
    void Update () {
        for (int i = _items.Count - 1; i >= 0; i--) {
            if ((Time.time - _startTimes[_items[i].gameObject]) / (_items[i].duration * 2.0f) > 1.0f) {
                GameObject g = _items[i].gameObject;
                _items.RemoveAt(i);
                Destroy(g);
            }
        }
    }
}
