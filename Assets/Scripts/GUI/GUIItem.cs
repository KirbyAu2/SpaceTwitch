using UnityEngine;
using System.Collections;

/*
 * The GUIItem class takes in data and makes a GUIItem
 */
public class GUIItem {
    private const float BEGIN_FADE_THRESH = .6f;

    public float xpos;
    public float ypos;
    public float starttime;
    public int dur;
    public string message;
    public GUIStyle customGuiStyle;
    public float currentAlpha = 1;

    /**
     * Takes in data and makes a GUIItem 
     * Set duration to -1 for unlimited showing
     */
    public GUIItem(float xposition, float yposition, string curmessage, GUIStyle cusGuiStyle, int duration = -1)
    {
        xpos = xposition;
        ypos = yposition;
        dur = duration;
        message = curmessage;
        customGuiStyle = cusGuiStyle;
    }

    public void update() {
        if (Time.time >= starttime + dur && dur != -1) {
            GUIManager.Instance.removeGUIItem(this);
        }
        if (Time.time >= starttime + (float)(dur) * BEGIN_FADE_THRESH && dur != -1) {
            float top = Time.time - (starttime + (float)(dur) * BEGIN_FADE_THRESH);
            float bottom = dur - dur * BEGIN_FADE_THRESH;
            currentAlpha = 1 - ((top) / (bottom));
            if (currentAlpha < 0) {
                currentAlpha = 0;
            }
        }
    }
}
