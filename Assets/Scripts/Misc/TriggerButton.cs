using UnityEngine;
using System.Collections;

public class TriggerButton : MonoBehaviour {
    public Texture2D buttonImage;

    private bool _pressed = false;

    public bool Pressed {
        get {
            return _pressed;
        }
    }

    void OnGUI() {
        Rect hitbox = new Rect(ScreenUtil.getPixelWidth(100), ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(250), ScreenUtil.getPixelWidth(150), ScreenUtil.getPixelHeight(150));
        _pressed = false;

#if UNITY_EDITOR && UNITY_IPHONE
        if (Input.GetMouseButton(0)) {
            Vector2 pos = Input.mousePosition;
            pos.y = Screen.height - pos.y;
            Debug.Log(pos);
            if (hitbox.Contains(pos)) {
                _pressed = true;
            }
        }
#elif UNITY_IPHONE
        for (int i = 0; i < Input.touchCount; i++) {
            Vector2 pos = Input.GetTouch(i).position;
            pos.y = Screen.height - pos.y;
            if(hitbox.Contains(pos)) {
                _pressed = true;
            }
        }
#endif

        if (buttonImage == null) {
            return;
        }
        GUI.DrawTexture(hitbox, buttonImage, ScaleMode.ScaleToFit);
    }
}
