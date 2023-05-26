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
                // �߂��̒��Ԃ�����
                // UpdateFindNeighbors();
                // �߂��̒��Ԃ̒��S�ֈړ�
                // UpdateBinding();
                // �߂��̒��Ԃ��痣���
                //UpdateSeparation();
                // �߂��̒��ԂƑ��x�����킹��
                //UpdateAlignment();
                // �ړ�
                // UpdateMove();

                // �v���C���[�̕��ֈړ�
                UpdateMoveToTarget();
                break;
            case MINION_MODE.MOVE_ENEMY:
                if (m_attackCollider.m_HitEnemyList.Count > 0)
                {
                    // �ł������ԋ߂��G
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
