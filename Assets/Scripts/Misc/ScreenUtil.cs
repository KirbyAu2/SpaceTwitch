using UnityEngine;
using System.Collections;

public class ScreenUtil {
    public const float DEFAULT_WIDTH = 1920.0f;

    public static float getPixelWidth(float x) {
        float newX = (x / DEFAULT_WIDTH) * Screen.width;
        if(GameManager.Instance.enableSeebright) {
            Debug.Log("wtf is going on!");
            newX /= 2.0f;
        }
        return newX;
    }

    public static float getPixelHeight(float x) {
        float newX = (x / DEFAULT_WIDTH) * Screen.width;
        return newX;
    }

    public static int ScreenWidth {
        get {
            if(GameManager.Instance == null) {
                return 0;
            }
            if(GameManager.Instance.enableSeebright) {
                return Screen.width / 2;
            }
            return Screen.width;
        }
    }

    public static int ScreenHeight {
        get {
            return Screen.height;
        }
    }
}
