using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ExplosionMat))]
public class Explosion : MonoBehaviour {
    private const float STANDARD_EXPLOSION_DURATION = 1.2f;
    private const float DEFAULT_ALPHA = 0.6f;

    private ExplosionMat _explosionMaterial;
    private float _duration;
    private float _startTime;

    void Start () {
        _explosionMaterial = GetComponent<ExplosionMat>();
        _explosionMaterial._alpha = DEFAULT_ALPHA;
        _explosionMaterial._heat = (Random.value * DEFAULT_ALPHA) + DEFAULT_ALPHA;

        _duration = Random.value * STANDARD_EXPLOSION_DURATION + STANDARD_EXPLOSION_DURATION;
        _startTime = Time.time;
    }

    void Update () {
        if(Time.time < _startTime + _duration) {
            Destroy (gameObject);
        }
    }
}
