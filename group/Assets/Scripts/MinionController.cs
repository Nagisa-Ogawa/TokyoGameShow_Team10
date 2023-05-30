using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinionController : MonoBehaviour
{
    [SerializeField]
    private List<Minion> m_minions = new List<Minion>();
    [SerializeField]
    private EnemyController m_enemyController = null;
    [SerializeField]
    private GameObject m_player= null;
    public GameObject m_Player {  get { return m_player; } private set { m_player = value; } }
    [SerializeField]
    private GameObject m_playerBack = null;
    public Minion.MINION_MODE m_mode { get; set; }
    public List<Minion> m_Minions { get { return m_minions; } private set { m_minions = value; }  }
    [SerializeField]
    private List<GameObject> m_minionPrefabList = new List<GameObject>();
    [SerializeField]
    private GameObject m_canvas= null;
    [SerializeField]
    private GameObject m_hpui= null;
    [SerializeField]
    private int m_minionMax = 30;
    [SerializeField]
    private GameObject m_hpUIParent = null;

    // レベルアップ系
    [SerializeField]
    private int m_level = 0;
    public int m_Level { get { return m_level; } private set { m_level = value; } }
    [SerializeField]
    private int m_experiencePoint = 0;
    public int m_LevelUpPoint = 0;
    [SerializeField]
    private List<int> m_levelPointList=new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance!=null&& GameManager.Instance.m_MinionLevelList.Count > 0)
        {
            CreateMinionStart();
            m_level = GameManager.Instance.m_Level;
            m_experiencePoint = GameManager.Instance.m_ExperiencePoint;
            m_LevelUpPoint=GameManager.Instance.m_LevelUpPoint;
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                var minion = Instantiate(m_minionPrefabList[i]);
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), 0.0f) + m_player.transform.position;
                minion.transform.position = pos;
                m_minions.Add(minion.GetComponent<MinionA>());
                // ui作成
                var hpui = Instantiate(m_hpui, m_hpUIParent.transform);
                minion.GetComponent<MinionA>().m_hpui = hpui;
                hpui.GetComponent<MinionHPUI>().m_minion = minion.GetComponent<MinionA>();
            }
        }
        m_mode = Minion.MINION_MODE.FOLLOW;
        ChangeMode(Minion.MINION_MODE.FOLLOW);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMinion(Enemy enemy)
    {
        // ミニオンにする
        var minionObj = Instantiate(m_minionPrefabList[(int)enemy.m_type]);
        var minion = minionObj.GetComponent<Minion>();
        minion.transform.position = enemy.transform.position;
        minion.m_mode = m_mode;
        if (m_enemyController.m_targetEnemy != null)
        {
            minion.m_targetEnemy = m_enemyController.m_targetEnemy;
            minion.m_target = m_enemyController.m_targetEnemy.gameObject;
        }
        SetStatus(minion);
        m_Minions.Add(minion);
        // ui作成
        var hpui = Instantiate(m_hpui, m_hpUIParent.transform);
        minion.m_hpui = hpui;
        hpui.GetComponent<MinionHPUI>().m_minion = minion;
        //if (enemy.m_type == Enemy.ENEMY_TYPE.LADYBIRD)
        //{
        //    // ミニオンにする
        //    var minionObj = Instantiate(m_minionA);
        //    var minion = minionObj.GetComponent<Minion>();
        //    minion.transform.position = enemy.transform.position;
        //    minion.m_mode = m_mode;
        //    if (m_enemyController.m_targetEnemy != null)
        //    {
        //        minion.m_targetEnemy = m_enemyController.m_targetEnemy;
        //        minion.m_target = m_enemyController.m_targetEnemy.gameObject;
        //    }
        //    SetStatus(minion);
        //    m_Minions.Add(minion);
        //    // ui作成
        //    var hpui = Instantiate(m_hpui, m_hpUIParent.transform);
        //    minion.m_hpui = hpui;
        //    hpui.GetComponent<MinionHPUI>().m_minion = minion;
        //}
        //else if(enemy.m_type==Enemy.ENEMY_TYPE.ANTS)
        //{
        //    // ミニオンにする
        //    var minionObj = Instantiate(m_minionB);
        //    var minion = minionObj.GetComponent<Minion>();
        //    minion.transform.position = enemy.transform.position;
        //    minion.m_mode = m_mode;
        //    if(m_enemyController.m_targetEnemy != null)
        //    {
        //        minion.m_targetEnemy = m_enemyController.m_targetEnemy;
        //        minion.m_target = m_enemyController.m_targetEnemy.gameObject;
        //    }
        //    SetStatus(minion);
        //    m_Minions.Add(minion);
        //    // ui作成
        //    var hpui = Instantiate(m_hpui, m_hpUIParent.transform);
        //    minion.m_hpui = hpui;
        //    hpui.GetComponent<MinionHPUI>().m_minion = minion;
        //}
        ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);
    }

    public void ChangeMode(Minion.MINION_MODE mode)
    {
        if (mode == Minion.MINION_MODE.ESCAPE && m_mode != Minion.MINION_MODE.MOVE_ENEMY) return;
        // すべてのミニオンを初期状態へ
        ResetMinion();

        m_mode = mode;
        foreach (Minion minion in m_Minions)
        {
            if (minion.m_mode == Minion.MINION_MODE.DEAD) continue;
            minion.m_mode = mode;
        }
        if (mode == Minion.MINION_MODE.FOLLOW)
        {
            ChangeTarget(m_playerBack);
        }
        if (mode==Minion.MINION_MODE.MOVE_ENEMY)
        {
            m_enemyController.FindTargetEnemy();
            if (m_enemyController.m_targetEnemy != null)
            {
                ChangeTarget(m_enemyController.m_targetEnemy.gameObject);
            }
            else
            {
                ChangeMode(Minion.MINION_MODE.FOLLOW);
            }
        }
    }

    public void ChangeTarget(GameObject gameObject)
    {
        if(m_mode==Minion.MINION_MODE.FOLLOW)
        {
            foreach (Minion minion in m_Minions)
            {
                minion.m_target = gameObject;
            }
        }
        else
        {
            foreach (Minion minion in m_Minions)
            {
                minion.m_target = gameObject;
                minion.m_targetEnemy = m_enemyController.m_targetEnemy;
            }
        }
    }

    public void ResetMinion()
    {
        foreach (Minion minion in m_Minions)
        {
            if (minion.m_mode == Minion.MINION_MODE.DEAD) continue;
            minion.StopMinionCoroutine();
            minion.m_nowTime = 0.0f;
            minion.m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
            minion.m_renderer.material.color = minion.m_color;
            minion.ResetShadow();
        }
    }

    public void RevivalMinion()
    {
        foreach (Minion minion in m_Minions)
        {
            if (minion.m_mode != Minion.MINION_MODE.DEAD)
            {
                if (minion.m_HP != minion.m_maxHP)
                {
                    minion.m_HP = minion.m_maxHP;
                }
            }
            else
            {
                // ここで死んでいたミニオンをdestroy
                Destroy(minion.gameObject);
            }
        }
    }

    public IEnumerator CheckAllDied()
    {
        foreach(var minion in m_Minions)
        {
            if (minion.m_mode != Minion.MINION_MODE.DEAD)
            {
                yield break;
            }
        }
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("GameOver");
    }

    public void AddExperiencePoint(int point)
    {
        m_experiencePoint += point;
        if (m_experiencePoint >= m_levelPointList[m_level]&&m_level<10)
        {
            m_level++;
            m_LevelUpPoint++;
        }
    }

    public void LevelUp(Minion.MINION_TYPE type,int point)
    {
        m_LevelUpPoint -= point;
        foreach(var minion in m_Minions)
        {
            if(minion.m_type== type)
            {
                minion.LevelUp();
            }
        }
    }

    public void SetStatus(Minion minion)
    {
        Minion target = null;
        foreach(var m in m_Minions)
        {
            if(minion.m_type==m.m_type)
            {
                target = m;
                break;
            }
        }
        if(target != null)
        {
            minion.SetStatus(target);
        }
    }

    public void CreateMinionStart()
    {
        var typeList = GameManager.Instance.m_MinionTypeList;
        var levelList = GameManager.Instance.m_MinionLevelList;
        for (int i = 0; i < typeList.Count; i++)
        {
            var minion = Instantiate(m_minionPrefabList[(int)typeList[i]]);
            Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), 0.0f) + m_player.transform.position;
            minion.transform.position = pos;
            m_minions.Add(minion.GetComponent<MinionA>());
            // ui作成
            var hpui = Instantiate(m_hpui, m_hpUIParent.transform);
            minion.GetComponent<MinionA>().m_hpui = hpui;
            minion.GetComponent<MinionA>().SetLevel(levelList[i]);
            hpui.GetComponent<MinionHPUI>().m_minion = minion.GetComponent<MinionA>();
        }
    }

    public void SaveMinionData()
    {
        var typeList= new List<Minion.MINION_TYPE>();
        var levelList=new List<int>();
        for(int i=0;i<m_Minions.Count;i++)
        {
            if (m_Minions[i].m_mode != Minion.MINION_MODE.DEAD)
            {
                typeList.Add(m_Minions[i].m_type);
                levelList.Add(m_Minions[i].m_Level);
            }
            else
            {
                Destroy(m_minions[i].gameObject);
            }
        }
        GameManager.Instance.UpdateData(m_level, m_LevelUpPoint, m_experiencePoint, typeList, levelList);
    }
}
