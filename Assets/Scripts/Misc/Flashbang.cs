using UnityEngine;
using System.Collections;

public class Flashbang : MonoBehaviour {
    public bool running = false;
    private float _duration = 0.0f;
    private float _startTime;
    private Texture2D _whiteBox;

    void Start () {
        _whiteBox = new Texture2D(1, 1);
        _whiteBox.SetPixel(1, 1, Color.white);
        _whiteBox.wrapMode = TextureWrapMode.Repeat;
        _whiteBox.Apply();
    }
	
    void Update () {
	
    }

    public void init(float duration) {
        running = true;
        _duration = duration;
        _startTime = Time.time;
    }

    public bool manualUpdate() {
        if ((Time.time - _startTime) / _duration >= 1.0f) {
            running = false;
        }
        return running;
    }

    void OnGUI() {
        if (!running) {
            return;
        }
        float percent = (Time.time - _startTime) / _duration;
        if (percent >= 1.0f) {
            percent = 1.0f;
        }
        Color c = new Color(1.0f,1.0f,1.0f, Mathf.Sin(percent * Mathf.PI));
        _whiteBox.SetPixel(1, 1, c);
        _whiteBox.wrapMode = TextureWrapMode.Repeat;
        _whiteBox.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _whiteBox);
    }
}
