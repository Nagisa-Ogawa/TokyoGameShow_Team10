using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackCollider : MonoBehaviour
{
    [SerializeField]
    private Minion m_minion = null;

    private List<Enemy> m_hitEnemyList = new List<Enemy>();
    public List<Enemy> m_HitEnemyList { get { return m_hitEnemyList; } private set { m_hitEnemyList = value; } }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "Enemy" && m_minion.m_mode == Minion.MINION_MODE.MOVE_ENEMY)
        //{
        //    if (m_minion.CheckCanAttack() == false) return;
        //    m_minion.m_targetEnemy = collision.gameObject.GetComponent<Enemy>();
        //    var rigidBody = m_minion.GetComponent<Rigidbody2D>();
        //    rigidBody.bodyType = RigidbodyType2D.Static;
        //    m_minion.m_mode = Minion.MINION_MODE.ATTACK;
        //    m_minion.StartAttack();
        //}

        if (collision.tag == "Enemy" &&
        m_HitEnemyList.Contains(collision.GetComponent<Enemy>()) == false)
        {
            m_HitEnemyList.Add(collision.GetComponent<Enemy>());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" &&
                m_HitEnemyList.Contains(collision.GetComponent<Enemy>()) == true)
        {
            //if(m_minion.m_mode!=Minion.MINION_MODE.MOVE_ENEMY&&
            //    m_minion.m_mode != Minion.MINION_MODE.ESCAPE &&
            //        m_minion.m_mode != Minion.MINION_MODE.FOLLOW)
            //{
            //    return;
            //}
            m_HitEnemyList.Remove(collision.GetComponent<Enemy>());
        }
    }

}
