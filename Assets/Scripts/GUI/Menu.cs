
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    public GUIStyle style;
    public Texture2D logo;
    private bool Options = false;
    private float volume = 0.5f;
    private float sensitivity = 0.5f;

    void Start () {
        style.fontSize = (int)ScreenUtil.getPixels(style.fontSize);
    }
	
    void Update () {
	
    }

    void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width - ScreenUtil.getPixels(logo.width)) / 2, ScreenUtil.getPixels(100), ScreenUtil.getPixels(logo.width), ScreenUtil.getPixels(logo.height)), logo);
        if(!Options){

        
            if (GUI.Button(new Rect(Screen.width / 2 - ScreenUtil.getPixels(150), Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Play Game", style)) {
                Application.LoadLevel(1);
            }
            if (GUI.Button(new Rect(Screen.width / 2 - ScreenUtil.getPixels(150), 3 * Screen.height / 5, ScreenUtil.getPixels(400), style.fontSize), "Options", style))
            {
                //print ("Option Button Pressed!");
                Options = true;
            }
        }
        if (Options)
        {
            GUI.Box(new Rect(Screen.width/2 - 3*Screen.width/16, 2* Screen.height/3 - 4*Screen.height/14, 3*Screen.width/8, 3*Screen.height/9), "Options");
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 3* Screen.height / 5 + 10, 100, 26), "back"))
            {
                Options = false;
            }

            GUILayout.BeginArea( new Rect (Screen.width/2 - Screen.width/8 , Screen.height/2 - 40, Screen.width/4, Screen.height - 100));
            GUILayout.Box("Volume");
            volume = GUILayout.HorizontalSlider(volume, 0.0f, 1.0f);
            AudioListener.volume = volume;
            GUILayout.Space(30);
            GUILayout.Box("Sensitivity");
            sensitivity = GUILayout.HorizontalSlider(sensitivity, 0.0f, 1.0f);
            //TO DO: Mouse Sensitivity
            GUILayout.EndArea();

        }
    }
}
