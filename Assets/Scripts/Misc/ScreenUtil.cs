using UnityEngine;
using System.Collections;

public class ScreenUtil {
    public const float DEFAULT_WIDTH = 1920.0f;

    public static float getPixels(float x) {
        float newX = (x / DEFAULT_WIDTH) * Screen.width;
        return newX;
    }

}
