using UnityEngine;
using System.Collections;

public class GUIItem {
    public float xpos;
    public float ypos;
    public float starttime;
    public int dur;
    public string message;
    public GUIStyle customGuiStyle;

    public GUIItem(float xposition, float yposition, float start, int duration, string curmessage, GUIStyle cusGuiStyle)
    {
        xpos = xposition;
        ypos = yposition;
        starttime = start;
        dur = duration;
        message = curmessage;
        customGuiStyle = cusGuiStyle;
    }
}
