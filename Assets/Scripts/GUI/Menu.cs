
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    public GUIStyle style;
    public Texture2D logo;

    private bool _displayOptions = false;

    void Start () {
        style.fontSize = (int)ScreenUtil.getPixels(style.fontSize);
    }
	
    void Update () {
	
    }

    void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width - ScreenUtil.getPixels(logo.width)) / 2 - ScreenUtil.getPixels(30), ScreenUtil.getPixels(100), ScreenUtil.getPixels(logo.width), ScreenUtil.getPixels(logo.height)), logo);
        if(!_displayOptions){
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Play Game", style)) {
                Application.LoadLevel(1);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), style.fontSize), "Tutorial", style)) {
                Application.LoadLevel(2);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(200), ScreenUtil.getPixels(400), style.fontSize), "Options", style))
            {
                _displayOptions = true;
            }
        }
        if (_displayOptions)
        {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(Screen.width/2 - 3*Screen.width/16, Screen.height/2 - ScreenUtil.getPixels(100), 3*Screen.width/8, ScreenUtil.getPixels(200)), "Options",style);
            GUI.color = prev;

            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Volume", style);
            AudioListener.volume = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), 
                ScreenUtil.getPixels(50)), AudioListener.volume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(150), ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400))/ 2, Screen.height / 2 + ScreenUtil.getPixels(230), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);

            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(200)) / 2, Screen.height - ScreenUtil.getPixels(150), ScreenUtil.getPixels(200), style.fontSize), "Back",style)) {
                _displayOptions = false;
            }
        }
    }
}
