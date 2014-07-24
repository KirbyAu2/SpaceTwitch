using UnityEngine;

public class JoystickButton
{
    public Texture Up, Over, Down;
    public string text;
    public Rect buttonRect;
    public bool isPressed, isFocused, enabled;

    public JoystickButton(Rect rect, string label)
    {
        enabled = true;

        text = label;

        buttonRect = rect;

        isPressed = false;
        isFocused = false;
    }

    public void Display()
    {
        if (enabled)
        {
            Up = (Texture)GUI.skin.button.normal.background;
            Over = (Texture)GUI.skin.button.hover.background;
            Down = (Texture)GUI.skin.button.active.background;

            if (!isFocused && !isPressed)
            {
                GUI.DrawTexture(buttonRect, Up);
                GUI.skin.label.normal.textColor = GUI.skin.button.normal.textColor;
                GUI.Label(buttonRect, text);
            }
            else if (isFocused && !isPressed)
            {
                GUI.DrawTexture(buttonRect, Over);
                GUI.skin.label.normal.textColor = GUI.skin.button.hover.textColor;
                GUI.Label(buttonRect, text);
            }
            else if (isFocused && isPressed)
            {
                GUI.DrawTexture(buttonRect, Down);
                GUI.skin.label.normal.textColor = GUI.skin.button.active.textColor;
                GUI.Label(buttonRect, text);
            }
        }
    }

    public void Focus(bool fo)
    {
        isFocused = fo;
    }
    public void Focus()
    {
        isFocused = true;
    }

    public bool Click()
    {
        if (isFocused)
        {
            isPressed = true;
            return true;
        }
        return false;
    }

    public void UnClick()
    {
        if (isPressed)
        {
            isPressed = false;
        }
    }
}