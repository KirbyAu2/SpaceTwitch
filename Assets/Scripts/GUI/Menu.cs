using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    public GUIStyle style;
    public Texture2D logo;

    void Start () {
        style.fontSize = (int)ScreenUtil.getPixels(style.fontSize);
    }
	
    void Update () {
	
    }

    void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width - ScreenUtil.getPixels(logo.width)) / 2, ScreenUtil.getPixels(100), ScreenUtil.getPixels(logo.width), ScreenUtil.getPixels(logo.height)), logo);
        if (GUI.Button(new Rect(Screen.width / 2 - ScreenUtil.getPixels(150), Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Play Game", style)) {
            Application.LoadLevel(1);
        }
    }
}
