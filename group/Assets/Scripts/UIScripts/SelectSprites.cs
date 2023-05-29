using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSprites : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;
    private int selectedIndex = 0;
    bool once = false;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Sprite‚Ì‘I‘ð
        if (!once)
        {
            if (horizontalInput > 0)
            {
                selectedIndex++;
                if (selectedIndex >= sprites.Length)
                    selectedIndex = 0;
                once = true;
            }
            else if (horizontalInput < 0)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = sprites.Length - 1;
                once = true;
            }
        }

        if (horizontalInput == 0)
            once = false;

        // ‘I‘ð‚µ‚½Sprite‚ð•\Ž¦
        image.sprite = sprites[selectedIndex];
    }
}