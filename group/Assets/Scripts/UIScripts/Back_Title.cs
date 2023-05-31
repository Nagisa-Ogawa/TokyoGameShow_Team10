using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_Title : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Title");
    }
}
