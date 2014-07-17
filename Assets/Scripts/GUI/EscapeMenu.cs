using UnityEngine;
using System.Collections;

public class EscapeMenu : MonoBehaviour {
    public GUIStyle style;

    public bool currentlyActive { get; private set; }

    private float currentTime;
    private bool _displayOptions = false;

    void Start () {
        style.fontSize = (int)(ScreenUtil.getPixelHeight(style.fontSize));
        currentTime = Time.timeScale;
    }

    public void display() {
        CameraController.currentCamera.setBlurShader(true);
        _displayOptions = false;
        currentTime = Time.timeScale;
        Time.timeScale = 0;
        Screen.lockCursor = false;
        currentlyActive = true;
    }

    public void exit() {
        CameraController.currentCamera.setBlurShader(false);
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
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                                    ScreenUtil.getPixelWidth(400), style.fontSize), "Resume Game", style)) {
                exit();
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), "Main Menu", style)) {
                Application.LoadLevel(0);
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), "Options", style)) {
                _displayOptions = true;
            }
        }
        if (_displayOptions) {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 - 
                ScreenUtil.getPixelHeight(100), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
            GUI.color = prev;
            GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "General Volume", style);
            AudioListener.volume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400),
                ScreenUtil.getPixelHeight(50)), AudioListener.volume, 0f, 1.0f);
            GUI.Label(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(600)) / 2, ScreenUtil.ScreenHeight / 2, 
                ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(600)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.effectsVolume, 0f, 1.0f);
            GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1400)) / 2, ScreenUtil.ScreenHeight / 2, 
                ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.musicVolume, 0f, 1.0f);
            GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), 
                "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);
            GameManager.Instance.UpdateMusicVolume();

            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2, 
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", style)) {
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
