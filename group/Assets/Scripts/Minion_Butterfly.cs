using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Butterfly : Minion
{
    // Start is called before the first frame update
    protected void Awake()
    {
        m_MinionController = GameObject.Find("MinionController").GetComponent<MinionController>();
        m_player = GameObject.Find("Player");
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_playerBack = GameObject.Find("PlayerBack");
        m_color = m_renderer.material.color;
    }
    protected void Start()
    {
        m_pos = transform.position;
        m_maxHP = m_HP;
        m_shadow.SetActive(false);
    }

    protected void Update()
    {
        m_beforPos = transform.position;
        switch (m_mode)
        {
            case MINION_MODE.FOLLOW:
                // 近くの仲間を検索
                // UpdateFindNeighbors();
                // 近くの仲間の中心へ移動
                // UpdateBinding();
                // 近くの仲間から離れる
                //UpdateSeparation();
                // 近くの仲間と速度を合わせる
                //UpdateAlignment();
                // 移動
                // UpdateMove();

                // プレイヤーの方へ移動
                UpdateMoveToTarget();
                break;
            case MINION_MODE.MOVE_ENEMY:
                if (m_attackCollider.m_HitEnemyList.Count > 0)
                {
                    // できたら一番近い敵
                    m_targetEnemy = m_attackCollider.m_HitEnemyList[0];
                    m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    m_rigidbody.velocity = Vector3.zero;
                    m_mode = MINION_MODE.WAIT_COOLDOWN;
                }
                else
                {
                    CheckReturnPlayer();
                    UpdateMoveToTarget();
                }
                break;
            case MINION_MODE.WAIT_COOLDOWN:
                if (CheckCanAttack())
                {
                    StartAttack();
                }
                break;
            case MINION_MODE.ATTACK:
                CheckReturnPlayer();
                break;
            case MINION_MODE.ESCAPE:
                if (m_targetEnemy == null) return;
                CheckReturnPlayer();
                if ((gameObject.transform.position - m_targetEnemy.transform.position).magnitude > m_escapeDistance)
                {
                    m_mode = MINION_MODE.WAIT;
                    //var rot = Quaternion.FromToRotation(Vector3.up, m_target.transform.position - transform.position);
                    //transform.rotation = rot;
                    m_rigidbody.velocity = Vector3.zero;
                    m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
                }
                else
                {
                    UpdateEscapeToTarget();
                }
                break;
            case MINION_MODE.WAIT:
                CheckReturnPlayer();
                break;
            case MINION_MODE.DAMAGE:
                break;
            case MINION_MODE.DEAD:
                break;
        }
    }
}
