using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField]
    private Enemy m_enemy = null;

    private List<GameObject> m_hitObjList = new List<GameObject>();
    public List<GameObject> m_HitObjList { get { return m_hitObjList; } private set { m_hitObjList = value; } }

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
        //if (collision.gameObject.tag == "Minion")
        //{
        //    if (m_enemy.m_mode != Enemy.ENEMY_MODE.MOVE_ATTACK) return;
        //    // çUåÇÇ÷
        //}
        if ((collision.tag == "Minion" || collision.tag == "Player" ) &&
        m_hitObjList.Contains(collision.gameObject) == false)
        {
            m_hitObjList.Add(collision.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.tag == "Minion" || collision.tag == "Player") &&
                m_hitObjList.Contains(collision.gameObject) == true)
        {
            m_hitObjList.Remove(collision.gameObject);
        }
    }

}
