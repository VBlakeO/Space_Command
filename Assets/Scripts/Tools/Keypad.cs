using UnityEngine;

public class Keypad : MonoBehaviour
{
    public SpriteRenderer keypad = null;
    public SpriteRenderer secondKeypad = null;
    public Sprite[] keypadSprite;
    public bool state = false;
    public bool doubleKeypad = false;

    void Start()
    {
        if (keypad)
            keypad.sprite = keypadSprite[state ? 0 : 1];
    }

    public void ChangeKeypadState()
    {
        state = !state;

        if (keypad)
            keypad.sprite = keypadSprite[state ? 0 : 1];

        if (secondKeypad)
            secondKeypad.sprite = keypadSprite[state ? 0 : 1];
    }
}
