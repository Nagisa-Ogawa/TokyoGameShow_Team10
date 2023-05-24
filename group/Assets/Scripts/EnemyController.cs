using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    // 現在のターゲット
    public Enemy m_targetEnemy { get; private set; }
    [SerializeField]
    private GameObject m_enemyPrefab;
    [SerializeField]
    private GameObject m_HPUI = null;
    [SerializeField]
    private GameObject canvas = null;
    [SerializeField]
    private Transform m_Parents = null;
    [SerializeField]
    private List<GameObject> m_enemyPrefabs = new List<GameObject>();
    // 敵のリスト
    private List<Enemy> m_enemyList =new List<Enemy>();
    public List<Enemy> m_EnemyList {  get { return m_enemyList; } private set { m_enemyList = value; } }
    private List<GameObject> m_enemyUIList = new List<GameObject>();
    [SerializeField]
    private Player m_player = null;
    [SerializeField]
    private MinionController m_minionController = null;
    [SerializeField]
    private int m_getMinionCount = 1;
    private int m_nowEnemyCount = 0;
    [SerializeField]
    private float m_attackDistance = 5.0f;
    [SerializeField]
    private GameObject m_hpUIParent = null;


    // Start is called before the first frame update
    void Start()
    {
        // エネミーをリストに格納
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            m_enemyList.Add(enemy.GetComponent<Enemy>());
            var hpui = Instantiate(m_HPUI, m_hpUIParent.transform);
            m_enemyUIList.Add(hpui);
            hpui.GetComponent<EnemyHPUI>().m_enemy = enemy.GetComponent<Enemy>();
            enemy.GetComponent<Enemy>().m_hpui = hpui;
        }
        //for (int i = 0; i < m_enemyMax; i++)
        //{
        //    var enemy = Instantiate(m_enemyPrefab);
        //    Vector3 pos = new Vector3(Random.Range(-20.0f, 20.0f), Random.Range(-20.0f, 20.0f), 0.0f);
        //    float angleZ = Random.Range(0, 360.0f);
        //    enemy.transform.position = pos;
        //    enemy.transform.eulerAngles = new Vector3(0.0f, 0.0f, angleZ);
        //    m_enemyList.Add(enemy.GetComponent<Enemy>());
        //}
    }

    public List<GameObject> CreateEnemyBoss(int enemyNo,int enemyCount)
    {
        var enemies=new List<GameObject>();
        for(int i=0;i<enemyCount; i++)
        {
            var enemyObj = Instantiate(m_enemyPrefabs[enemyNo]);
            var enemy=enemyObj.GetComponent<Enemy>();
            var hpui = Instantiate(m_HPUI, m_hpUIParent.transform);
            hpui.GetComponent<EnemyHPUI>().m_enemy = enemy.GetComponent<Enemy>();
            enemy.GetComponent<Enemy>().m_hpui = hpui;
            enemy.isReviv = false;
            m_enemyList.Add(enemy);
            m_enemyUIList.Add(hpui);
            enemies.Add(enemyObj);
        }
        return enemies;
    }

    public void FindTargetEnemy()
    {
        Enemy target = null;
        m_targetEnemy = null;
        float minDistance = m_attackDistance;
        foreach(Enemy enemy in m_enemyList)
        {
            if (enemy.m_mode == Enemy.ENEMY_MODE.DEAD) continue;
            if (minDistance >= (m_player.transform.position - enemy.transform.position).magnitude)
            {
                target = enemy;
                minDistance = (m_player.transform.position - enemy.transform.position).magnitude;
            }
        }
        if(target !=null)
        {
            m_targetEnemy= target;
        }
    }

    public void CheckBecomeMinion(Enemy enemy)
    {
        m_nowEnemyCount++;
        if(m_nowEnemyCount>=m_getMinionCount)
        {
            m_nowEnemyCount= 0;
            m_minionController.CreateMinion(enemy);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void ActiveAll()
    {
        foreach(var enemy in m_enemyList)
        {
            if (enemy.m_mode == Enemy.ENEMY_MODE.DEAD&&enemy.isReviv)
            {
                enemy.gameObject.SetActive(true);
                enemy.RevivalEnemy();
                enemy.m_hpui.SetActive(true);
            }
        }
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("GameClear");
    }
}
