using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField]
    private Enemy m_enemy = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Minion")
        {
            if (m_enemy.m_mode != Enemy.ENEMY_MODE.MOVE_ATTACK) return;
            // çUåÇÇ÷
            m_enemy.m_targetMinion = collision.gameObject.GetComponent<Minion>();
            var rigidBody = m_enemy.GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            rigidBody.velocity = Vector2.zero;
            m_enemy.m_mode = Enemy.ENEMY_MODE.SETUP_ATTACK;
        }
    }

}
