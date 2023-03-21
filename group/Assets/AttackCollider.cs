using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField]
    private Minion m_minion = null;
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
        if (collision.gameObject.tag == "Enemy" && m_minion.m_mode == Minion.MINION_MODE.MOVE_ENEMY)
        {
            if (m_minion.CheckCanAttack() == false) return;
            m_minion.m_targetEnemy = collision.gameObject.GetComponent<Enemy>();
            var rigidBody=m_minion.GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            m_minion.m_mode = Minion.MINION_MODE.ATTACK;
            m_minion.StartAttack();
        }
    }
}
