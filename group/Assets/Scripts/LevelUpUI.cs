using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{

    [SerializeField]
    private GameObject m_levelUpUI = null;
    [SerializeField]
    private MinionController m_minionController = null;
    [SerializeField]
    private List<Button> m_buttonList = new List<Button>();
    [SerializeField]
    private List<GameObject> m_levelUpFrameList = new List<GameObject>();
    [SerializeField]
    private List<TextMeshProUGUI> m_levelTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_plusLevelTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_HPTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_plusHPTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_DamageTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_plusDamageTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_SpeedTextList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> m_plusSpeedTextList = new List<TextMeshProUGUI>();

    private List<int> m_levelList = new List<int>();
    private List<int> m_HPList= new List<int>();
    private List<int> m_damageList= new List<int>();
    private List<float> m_speedList = new List<float>();

    private List<int> m_plusLevelList= new List<int>();
    private List<int> m_plusHPList= new List<int>();
    private List<int> m_plusDamageList= new List<int>();
    private List<float> m_plusSpeedList= new List<float>();

    private int m_nowFrame = 0;

    [SerializeField]
    private List<bool> m_minionTypeList = new List<bool>();
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            m_levelList.Add(0);
            m_HPList.Add(0);
            m_damageList.Add(0);
            m_speedList.Add(0.0f);

            m_plusLevelList.Add(1);
            m_plusHPList.Add(0);
            m_plusDamageList.Add(0);
            m_plusSpeedList.Add(0.0f);
        }
        HideAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        CheckEnemyType();
        SetFirstFrame();
        ShowButton();
        GetStatus();
        ShowStatus();
        if(m_minionController.m_LevelUpPoint>0)
        {
            ShowPlusStatus(m_nowFrame);
            m_levelUpFrameList[m_nowFrame].SetActive(true);
        }
        m_levelUpUI.SetActive(true);
    }

    void CheckEnemyType()
    {
        foreach(var minion in m_minionController.m_Minions)
        {
            if (m_minionTypeList[(int)minion.m_type] == false)
            {
                m_minionTypeList[(int)minion.m_type] = true;
            }
        }
    }

    void SetFirstFrame()
    {
        if(m_minionController.m_LevelUpPoint==0)
        {
            m_nowFrame = -1;
            return;
        }
        for (int i = 0; i < m_minionTypeList.Count; i++)
        {
            if (m_minionTypeList[i] == true)
            {
                m_nowFrame = i;
                return;
            }
        }
    }

    void GetStatus()
    {
        for (int i = 0; i < m_minionTypeList.Count; i++)
        {
            if (m_minionTypeList[i] == true)
            {
                Minion target = null;
                foreach(var minion in m_minionController.m_Minions)
                {
                    if (minion.m_type == (Minion.MINION_TYPE)i)
                    {
                        target = minion;
                        break;
                    }
                }
                m_levelList[i] = target.m_Level;
                m_HPList[i] = target.m_maxHP;
                m_damageList[i] = target.m_Damage;
                m_speedList[i] = target.m_Speed;

                m_plusHPList[i] = target.m_AddHp;
                m_plusDamageList[i] = target.m_AddDamage;
                m_plusSpeedList[i]= target.m_AddSpeed;
            }
            else
            {
                m_levelList[i] = 0;
                m_HPList[i] = 0;
                m_damageList[i] = 0;
                m_speedList[i] = 0.0f;
            }
        }
    }

    void ShowStatus()
    {
        for (int i = 0; i < m_minionTypeList.Count; i++)
        {
            ChangeLevel(m_levelTextList[i], m_levelList[i] );
            ChangeHP(m_HPTextList[i], m_HPList[i]);
            ChangeDamage(m_DamageTextList[i], m_damageList[i]);
            ChangeSpeed(m_SpeedTextList[i], m_speedList[i]);
        }
    }

    void ChangeLevel(TextMeshProUGUI text,int level)
    {
        text.text = "Lv " + level;
    }

    void ChangeHP(TextMeshProUGUI text,int HP)
    {
        text.text = "HP : " + HP;
    }

    void ChangeDamage(TextMeshProUGUI text, int damage)
    {
        text.text = "POWER : " + damage;
    }

    void ChangeSpeed(TextMeshProUGUI text, float speed)
    {
        text.text = "SPEED : " + speed;
    }

    void ShowButton()
    {
        if(m_minionController.m_LevelUpPoint==0)
        {
            foreach(var button in m_buttonList)
            {
                button.interactable= false;
            }
            return;
        }
        for(int i=0;i<m_minionTypeList.Count;i++)
        {
            if (m_minionTypeList[i] == true)
            {
                m_buttonList[i].interactable = true;
            }
        }
    }

    void ShowPlusStatus(int num)
    {
        ChangePlusLevel(m_plusLevelTextList[num], m_plusLevelList[num]);
        ChangePlusHP(m_plusHPTextList[num], m_plusHPList[num]);
        ChangePlusDamage(m_plusDamageTextList[num], m_plusDamageList[num]);
        ChangePlusSpeed(m_plusSpeedTextList[num], m_plusSpeedList[num]);
        m_plusLevelTextList[num].gameObject.SetActive(true);
        m_plusHPTextList[num].gameObject.SetActive(true);
        m_plusDamageTextList[num].gameObject.SetActive(true) ;
        m_plusSpeedTextList[num].gameObject .SetActive(true) ;
    }

    void HidePlusStatus(int num)
    {
        m_plusLevelTextList[num].gameObject.SetActive(false);
        m_plusHPTextList[num].gameObject.SetActive(false);
        m_plusDamageTextList[num].gameObject.SetActive(false);
        m_plusSpeedTextList[num].gameObject.SetActive(false);
    }

    void HideButton()
    {
        foreach (var button in m_buttonList)
        {
            button.interactable = false;
        }
    }

    void HideFrame()
    {
        m_levelUpFrameList[m_nowFrame].SetActive(false);
        m_nowFrame = -1;
    }

    void ChangePlusLevel(TextMeshProUGUI text,int addLevel)
    {
        text.text = "+" + addLevel;
    }

    void ChangePlusHP(TextMeshProUGUI text,int addHP)
    {
        text.text = "+" + addHP;
    }

    void ChangePlusDamage(TextMeshProUGUI text,int addDamage)
    {
        text.text = "+" + addDamage;
    }

    void ChangePlusSpeed(TextMeshProUGUI text, float addSpeed)
    {
        text.text = "+" + (int)addSpeed;
    }

    public void Move_Up()
    {
        if (m_nowFrame == -1)
        {
            return;
        }
        if(m_nowFrame>=1)
        {
            int moveNum = -1;
            for(int i = m_nowFrame-1; i >= 0; i--)
            {
                if (m_minionTypeList[i] == true)
                {
                    moveNum = i;
                }
            }
            if(moveNum==-1)
            {
                return;
            }
            // 現在の枠を描画しない
            m_levelUpFrameList[m_nowFrame].SetActive(false);
            HidePlusStatus(m_nowFrame);
            // 次の枠へ
            m_nowFrame = moveNum;
            // 次の枠を表示
            m_levelUpFrameList[m_nowFrame].SetActive(true);
            ShowPlusStatus(m_nowFrame);
        }
        else
        {
            return;
        }
    }

    public void Move_Down()
    {
        if (m_nowFrame == -1)
        {
            return;
        }
        if (m_nowFrame <= 5)
        {
            int moveNum = -1;
            for (int i = m_nowFrame + 1; i <= 6; i++)
            {
                if (m_minionTypeList[i] == true)
                {
                    moveNum = i;
                }
            }
            if (moveNum == -1)
            {
                return;
            }
            // 現在の枠を描画しない
            m_levelUpFrameList[m_nowFrame].SetActive(false);
            HidePlusStatus(m_nowFrame);
            // 次の枠へ
            m_nowFrame = moveNum;
            // 次の枠を表示
            m_levelUpFrameList[m_nowFrame].SetActive(true);
            ShowPlusStatus(m_nowFrame);
        }
        else
        {
            return;
        }
    }

    public void Decide()
    {
        if(m_minionController.m_LevelUpPoint==0)
        {
            return;
        }
        // ステータスを反映
        m_minionController.LevelUp((Minion.MINION_TYPE)m_nowFrame);
        // テキストを更新
        GetStatus();
        ShowStatus();
        // もう一度レベルアップできるか確認
        if(m_minionController.m_LevelUpPoint==0)
        {
            HidePlusStatus(m_nowFrame);
            HideButton();
            HideFrame();
        }
    }

    public void Uninit()
    {
        m_nowFrame = 0;
        HideAll();
        m_levelUpUI.SetActive(false);
    }


    void HideAll()
    {
        m_nowFrame = -1;
        foreach (var frame in m_levelUpFrameList)
        {
            frame.SetActive(false);
        }
        foreach (var level in m_plusLevelTextList)
        {
            level.gameObject.SetActive(false);
        }
        foreach (var hp in m_plusHPTextList)
        {
            hp.gameObject.SetActive(false);
        }
        foreach (var damage in m_plusDamageTextList)
        {
            damage.gameObject.SetActive(false);
        }
        foreach (var speed in m_plusSpeedTextList)
        {
            speed.gameObject.SetActive(false);
        }
        foreach (var button in m_buttonList)
        {
            button.interactable = false;
        }
    }

}
