using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text;
    [SerializeField]
    private MinionController m_minionController = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_minionController.m_mode)
        {
            case Minion.MINION_MODE.FOLLOW:
                m_text.text = ("FOLLOW");
                break;
            case Minion.MINION_MODE.MOVE_ENEMY:
                m_text.text = ("ATTACK");
                break;
            case Minion.MINION_MODE.ESCAPE:
                m_text.text = ("ESCAPE");
                break;
        }
    }
}
