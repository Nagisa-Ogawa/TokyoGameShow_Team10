using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField]
    private Enemy m_enemy = null;

    private List<Minion> m_hitMinionList = new List<Minion>();
    public List<Minion> m_HitMinionList { get { return m_hitMinionList; } private set { m_hitMinionList = value; } }

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
        //    // �U����
        //}
        if (collision.tag == "Minion" &&
        m_hitMinionList.Contains(collision.GetComponent<Minion>()) == false)
        {
            m_hitMinionList.Add(collision.GetComponent<Minion>());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Minion" &&
                m_hitMinionList.Contains(collision.GetComponent<Minion>()) == true)
        {
            m_hitMinionList.Remove(collision.GetComponent<Minion>());
        }
    }

}
