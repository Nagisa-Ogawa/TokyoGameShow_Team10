using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // シングルトン
    private static GameManager instance;
    public static GameManager Instance {
        get {
            if (null == instance)
            {
                instance = (GameManager)FindObjectOfType(typeof(GameManager));
                if (null == instance) Debug.Log(" ゲームマネージャーのインスタンスを取得できませんでした ");
            }
            return instance;
        }
    }
    public int m_Level { get; private set; }
    public int m_LevelUpPoint { get; private set; }
    public int m_ExperiencePoint { get; private set; }
    private List<Minion.MINION_TYPE> m_minonTypeList = new List<Minion.MINION_TYPE>();
    public List<Minion.MINION_TYPE> m_MinionTypeList { get { return m_minonTypeList; } set { m_minonTypeList = value; } }
    private List<int> m_minionLevelList = new List<int>();
    public List<int > m_MinionLevelList { get { return m_minionLevelList; } set { m_minionLevelList = value; } }


    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("GameManager"); //タグで判別
        if (1 < obj.Length) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    public void UpdateData(int level,int levelUp,int experience, 
        List<Minion.MINION_TYPE> minionTypeList,List<int> minionLevelList)
    {
        // 一度データを削除
        m_minionLevelList.Clear();
        m_minonTypeList.Clear();
        m_Level = level;
        m_LevelUpPoint = levelUp;
        m_ExperiencePoint = experience;
        for(int i=0;i<minionTypeList.Count;i++)
        {
            m_minonTypeList.Add(minionTypeList[i]);
            m_minionLevelList.Add(minionLevelList[i]);
        }
    }
}
