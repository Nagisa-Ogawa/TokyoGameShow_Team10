using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
    [SerializeField]
    private Enemy m_enemy = null;
    private List<GameObject> m_hitObjList = new List<GameObject>();
    public List<GameObject> m_HitObjList { get { return m_hitObjList; } set { m_hitObjList = value; } }
    [SerializeField]
    private float m_rangeDistance = 3.0f;
    [SerializeField]
    private int m_rangeAttackDamage = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetRange(GameObject target)
    {
        transform.position += (target.transform.position - transform.position).normalized * m_rangeDistance;
        var rot = Quaternion.FromToRotation(Vector3.up,(target.transform.position-transform.position) );
        transform.rotation = rot;
    }

    public void DeleteList()
    {
        Debug.Log(m_hitObjList.Count + "人に攻撃しました");
        m_hitObjList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.name + "と当たった");
        // まだ一度も当たってないなら
        if ((collision.tag=="Minion"||collision.tag=="Player")&&
                m_enemy.m_mode == Enemy.ENEMY_MODE.RANGE_ATTACK&&
                    m_hitObjList.Contains(collision.gameObject)==false)
        {
            // ダメージ判定
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().Damage(m_rangeAttackDamage);
            }
            else
            {
                collision.GetComponent<Minion>().Damage(m_rangeAttackDamage, m_enemy);
            }
            m_hitObjList.Add(collision.gameObject);
        }
        //if ((collision.tag == "Minion" || collision.tag == "Player") &&
        //        m_hitObjList.Contains(collision.GetComponent<GameObject>()) == false)
        //{
        //    m_hitObjList.Add(collision.gameObject);
        //    Debug.Log("入った　：　" + m_HitObjList.Count);
        //}
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if ((collision.tag == "Minion" || collision.tag == "Player") &&
    //            m_hitObjList.Contains(collision.GetComponent<GameObject>()) == true)
    //    {
    //        m_hitObjList.Remove(collision.gameObject);
    //        Debug.Log("でた　：　" + m_HitObjList.Count);
    //    }
    //}
}
