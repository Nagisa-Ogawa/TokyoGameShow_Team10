using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Go_Stage1 : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Stage1");
    }
}
