using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Yoko_SelectButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] buttons;
    private int selectedIndex = 0;
    bool once = false;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool submitInput = Input.GetButtonDown("Submit");

        if (!once)
        {
            // �{�^���̑I��
            if (horizontalInput < 0)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = buttons.Length - 1;
                once = true;
            }
            else if (horizontalInput > 0)
            {
                selectedIndex++;
                if (selectedIndex >= buttons.Length)
                    selectedIndex = 0;
                once = true;
            }
        }

        if (horizontalInput == 0)
            once = false;

        // �{�^���̑I����Ԃ��X�V
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
                buttons[i].Select();
            else
                buttons[i].OnDeselect(null);
        }

        // A�{�^���őI�������{�^���̏��������s
        if (submitInput)
            buttons[selectedIndex].onClick.Invoke();
    }
}
