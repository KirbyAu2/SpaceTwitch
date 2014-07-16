using UnityEngine;
using System.Collections;

public class EscapeMenu : MonoBehaviour {
    public GUIStyle style;

    public bool currentlyActive { get; private set; }

    private float currentTime;
    private bool _displayOptions = false;
    private BlurEffect _blur;

    void Start () {
        style.fontSize = (int)(ScreenUtil.getPixels(style.fontSize));
        currentTime = Time.timeScale;
    }

    public void display() {
        if (_blur == null) {
            _blur = CameraController.currentCamera.gameObject.GetComponent<BlurEffect>();
        }
        _displayOptions = false;
        _blur.enabled = true;
        currentTime = Time.timeScale;
        Time.timeScale = 0;
        Screen.lockCursor = false;
        currentlyActive = true;
    }

    public void exit() {
        _blur.enabled = false;
        Time.timeScale = currentTime;
        currentlyActive = false;
        Screen.lockCursor = true;
    }

	
    void Update () {

    }

    void OnGUI() {
        if (!currentlyActive) {
            return;
        }
        if (!_displayOptions) {
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Resume Game", style)) {
                exit();
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), style.fontSize), "Main Menu", style)) {
                Application.LoadLevel(0);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(200), ScreenUtil.getPixels(400), style.fontSize), "Options", style)) {
                _displayOptions = true;
            }
        }
        if (_displayOptions) {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 - ScreenUtil.getPixels(100), 3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Options", style);
            GUI.color = prev;

            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "General Volume", style);
            AudioListener.volume = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400),
                ScreenUtil.getPixels(50)), AudioListener.volume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width + ScreenUtil.getPixels(600)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Effects Volume", style);
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((Screen.width + ScreenUtil.getPixels(600)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.effectsVolume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(1400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Music Volume", style);
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(1400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.musicVolume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(150), ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(230), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);

            GameManager.Instance.UpdateMusicVolume();

            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(200)) / 2, Screen.height - ScreenUtil.getPixels(150), ScreenUtil.getPixels(200), style.fontSize), "Back", style)) {
                _displayOptions = false;
                // update sensitivity when options are closed
                foreach (Player player in GameManager.Instance.CurrentPlayerShips)
                {
                    player.UpdateSensitivity();
                }
            }
        }
    }
}
