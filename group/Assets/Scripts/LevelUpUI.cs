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
    private TextMeshProUGUI m_totalPointText = null;
    [SerializeField]
    private TextMeshProUGUI m_plusPointText = null;

    private List<int> m_levelList = new List<int>();
    private List<int> m_HPList= new List<int>();
    private List<int> m_damageList= new List<int>();

    private List<int> m_plusLevelList= new List<int>();
    private List<int> m_plusHPList= new List<int>();
    private List<int> m_plusDamageList= new List<int>();

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

            m_plusLevelList.Add(1);
            m_plusHPList.Add(0);
            m_plusDamageList.Add(0);
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
        GetStatus();
        ShowButton();
        SetFirstFrame();
        ShowStatus();
        GetTotalPoint();
        if(m_minionController.m_LevelUpPoint>0)
        {
            ShowPlusStatus(m_nowFrame);
            ShowPlusTotalPoint();
            m_levelUpFrameList[m_nowFrame].SetActive(true);
        }
        m_levelUpUI.SetActive(true);
    }

    void GetTotalPoint()
    {
        m_totalPointText.text = "POINT : " + m_minionController.m_LevelUpPoint;
    }

    void ShowPlusTotalPoint()
    {
        m_plusPointText.gameObject.SetActive(true);
        if (m_levelList[m_nowFrame] < 6)
        {
            m_plusPointText.text = "-1";
        }
        else
        {
            m_plusPointText.text = "-2";
        }
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
            if (m_minionTypeList[i] == true && m_buttonList[i].interactable==true)
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

                if (m_levelList[i] < 10)
                {
                    m_plusHPList[i] = target.m_StatusList[target.m_Level].HP - target.m_StatusList[target.m_Level - 1].HP;
                    m_plusDamageList[i] = target.m_StatusList[target.m_Level].Attack - target.m_StatusList[target.m_Level - 1].Attack;
                }
            }
            else
            {
                m_levelList[i] = 0;
                m_HPList[i] = 0;
                m_damageList[i] = 0;
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
            if(m_levelList[i] > 5 && m_minionController.m_LevelUpPoint < 2)
            {
                m_buttonList[i].interactable = false;
            }
            else if (m_levelList[i] == 10)
            {
                m_buttonList[i].interactable = false;
            }
        }
    }

    void ShowPlusStatus(int num)
    {
        ChangePlusLevel(m_plusLevelTextList[num], m_plusLevelList[num]);
        ChangePlusHP(m_plusHPTextList[num], m_plusHPList[num]);
        ChangePlusDamage(m_plusDamageTextList[num], m_plusDamageList[num]);
        m_plusLevelTextList[num].gameObject.SetActive(true);
        m_plusHPTextList[num].gameObject.SetActive(true);
        m_plusDamageTextList[num].gameObject.SetActive(true) ;
    }

    void HidePlusStatus(int num)
    {
        m_plusLevelTextList[num].gameObject.SetActive(false);
        m_plusHPTextList[num].gameObject.SetActive(false);
        m_plusDamageTextList[num].gameObject.SetActive(false);
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
                if (m_buttonList[i].interactable)
                {
                    moveNum = i;
                    break;
                }
            }
            if(moveNum==-1)
            {
                return;
            }
            // åªç›ÇÃògÇï`âÊÇµÇ»Ç¢
            m_levelUpFrameList[m_nowFrame].SetActive(false);
            HidePlusStatus(m_nowFrame);
            // éüÇÃògÇ÷
            m_nowFrame = moveNum;
            // éüÇÃògÇï\é¶
            m_levelUpFrameList[m_nowFrame].SetActive(true);
            ShowPlusStatus(m_nowFrame);
            ShowPlusTotalPoint();
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
                if (m_buttonList[i].interactable)
                {
                    moveNum = i;
                    break;
                }
            }
            if (moveNum == -1)
            {
                return;
            }
            // åªç›ÇÃògÇï`âÊÇµÇ»Ç¢
            m_levelUpFrameList[m_nowFrame].SetActive(false);
            HidePlusStatus(m_nowFrame);
            // éüÇÃògÇ÷
            m_nowFrame = moveNum;
            // éüÇÃògÇï\é¶
            m_levelUpFrameList[m_nowFrame].SetActive(true);
            ShowPlusStatus(m_nowFrame);
            ShowPlusTotalPoint();
        }
        else
        {
            return;
        }
    }

    public void Decide()
    {
        int beferFrame = -1;
        if(m_minionController.m_LevelUpPoint==0)
        {
            return;
        }
        int point = 0;
        if (m_levelList[m_nowFrame] < 6)
        {
            point = 1;
        }
        else
        {
            point = 2;
        }
        beferFrame = m_nowFrame;
        // ÉXÉeÅ[É^ÉXÇîΩâf
        m_minionController.LevelUp((Minion.MINION_TYPE)m_nowFrame,point);
        HideAll();
        // ÉeÉLÉXÉgÇçXêV
        CheckEnemyType();
        GetStatus();
        ShowButton();
        if (m_buttonList[beferFrame].interactable==false) 
        {
            SetFirstFrame();
        }
        else
        {
            m_nowFrame = beferFrame;
        }
        ShowStatus();
        GetTotalPoint();
        if (m_minionController.m_LevelUpPoint > 0)
        {
            ShowPlusStatus(m_nowFrame);
            ShowPlusTotalPoint();
            m_levelUpFrameList[m_nowFrame].SetActive(true);
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
        foreach (var button in m_buttonList)
        {
            button.interactable = false;
        }
        m_plusPointText.gameObject.SetActive(false);
    }

}
