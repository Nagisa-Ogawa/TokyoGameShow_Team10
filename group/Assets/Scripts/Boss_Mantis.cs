using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(Enemy))]
#endif

public class Boss_Mantis : Enemy
{
    public enum BOSS_MANTIS_MODE {
        WAIT,
        MOVE_ATTACK,
        WAIT_RANGE_ATTACKA_COOLDWON,
        WAIT_RANGE_ATTACKB_COOLDOWN,
        SETUP_RANGE_ATTACKA,
        SETUP_RANGE_ATTACKB,
        RANGE_ATTACKA,
        RANGE_ATTACKB,
        AFTER_ATTACK,
        CREATE_ENEMY,
        MOVE_TERRITORY,
        DEAD,
    }

    public new BOSS_MANTIS_MODE m_mode { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        m_mode = BOSS_MANTIS_MODE.WAIT;
        m_rangeColor = m_rangeRenderere.material.color;
        Color color = m_rangeRenderere.material.color;
        color.a = 0.0f;
        m_rangeRenderere.material.color = color;
        m_rangeLayerNo = m_rangeAttack.gameObject.layer;
        m_rangeAttack.gameObject.layer = 12;
        m_rangeAttack.gameObject.SetActive(false);
        m_territoryPos = gameObject.transform.position;
        m_maxHP = m_HP;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(m_mode);
        switch (m_mode)
        {
            case BOSS_MANTIS_MODE.WAIT:
                // テリトリーの外へ出たらテリトリーへ戻る
                if (CheckTerritory())
                {
                    // 移動方向を設定
                    if (CheckFolding())
                    {
                        transform.position = m_territoryPos + (Vector3)(m_moveDirection.normalized * m_moveDistance);
                        m_moveDirection.x *= -1.0f;
                        m_moveDirection.y *= -1.0f;
                    }
                    // 敵がテリトリーに侵入したら迎撃開始
                    if (FindTarget())
                    {
                        m_mode = BOSS_MANTIS_MODE.MOVE_ATTACK;
                    }
                }
                else
                {
                    m_mode = BOSS_MANTIS_MODE.MOVE_TERRITORY;
                }
                break;
            case BOSS_MANTIS_MODE.MOVE_ATTACK:
                if (m_target == null)
                {
                    ResetEnemy();
                    break;
                }
                // 攻撃検知エリア内にミニオンがいるなら
                if (m_enemyAttack.m_HitObjList.Count > 0)
                {
                    // できたら自分から一番近いやつを選ぶ
                    // m_enemy.m_targetMinion = collision.gameObject.GetComponent<Minion>();
                    if (m_target.tag == "Minion")
                    {
                        m_targetType = TARGET_TYPE.MINION;
                        m_targetMinion = m_target.GetComponent<Minion>();
                    }
                    else
                    {
                        m_targetType = TARGET_TYPE.PLAYER;
                    }

                    m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    m_rigidbody.velocity = Vector3.zero;
                        m_mode = BOSS_MANTIS_MODE.WAIT_RANGE_ATTACKA_COOLDWON;
                }
                else
                {
                    // テリトリーの外へ出たらテリトリーへ戻る
                    if (CheckTerritory() || (m_targetType == TARGET_TYPE.MINION && m_targetMinion.m_mode != Minion.MINION_MODE.ESCAPE))
                    {
                        // ターゲットの方へ
                        UpdateMoveToTarget();
                    }
                    else
                    {
                        m_mode = BOSS_MANTIS_MODE.MOVE_TERRITORY;
                    }
                }
                break;
            case BOSS_MANTIS_MODE.WAIT_RANGE_ATTACKA_COOLDWON:
                // 攻撃可能なら攻撃へ
                if (CheckCanRangeAttack())
                {
                    m_mode = BOSS_MANTIS_MODE.SETUP_RANGE_ATTACKA;
                    StartRangeAttack();
                }
                break;
            case BOSS_MANTIS_MODE.WAIT_RANGE_ATTACKB_COOLDOWN:
                // 攻撃可能なら攻撃へ
                if (CheckCanRangeAttack())
                {
                    m_mode = BOSS_MANTIS_MODE.SETUP_RANGE_ATTACKB;
                    StartRangeAttack();
                }
                break;
            case BOSS_MANTIS_MODE.SETUP_RANGE_ATTACKA:
                break;
            case BOSS_MANTIS_MODE.SETUP_RANGE_ATTACKB:
                break;
            case BOSS_MANTIS_MODE.RANGE_ATTACKA:
                break;
            case BOSS_MANTIS_MODE.RANGE_ATTACKB:

                break;
            case BOSS_MANTIS_MODE.MOVE_TERRITORY:
                // 攻撃検知エリア内にミニオンがいるなら
                if (m_enemyAttack.m_HitObjList.Count > 0)
                {
                    // できたら自分から一番近いやつを選ぶ
                    // m_enemy.m_targetMinion = collision.gameObject.GetComponent<Minion>();
                    m_target = m_enemyAttack.m_HitObjList[0];
                    if (m_target.tag == "Minion")
                    {
                        m_targetType = TARGET_TYPE.MINION;
                        m_targetMinion = m_target.GetComponent<Minion>();
                    }
                    else
                    {
                        m_targetType = TARGET_TYPE.PLAYER;
                    }

                    m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    m_rigidbody.velocity = Vector3.zero;
                    // m_mode = ENEMY_MODE.WAIT_ATTACK_COOLDWON;
                    m_mode = BOSS_MANTIS_MODE.WAIT_RANGE_ATTACKA_COOLDWON;
                }
                else
                {
                    // テリトリーに侵入したら迎撃開始
                    if (FindTarget())
                    {
                        m_mode = BOSS_MANTIS_MODE.MOVE_ATTACK;
                    }
                    else
                    {
                        if ((transform.position - m_territoryPos).magnitude <= 0.1f)
                        {
                            transform.position = m_territoryPos;
                            m_mode = BOSS_MANTIS_MODE.WAIT;
                            m_rigidbody.velocity = Vector2.zero;
                        }
                        UpdateMoveToTerritory();
                    }
                }
                break;
            case BOSS_MANTIS_MODE.DEAD:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (m_mode)
        {
            case BOSS_MANTIS_MODE.WAIT:
                //var rot = Quaternion.FromToRotation(Vector3.up, m_moveDirection);
                //transform.rotation = rot;
                m_rigidbody.velocity = m_moveDirection;
                break;
            case BOSS_MANTIS_MODE.MOVE_ATTACK:
                UpdateMove();
                break;
            case BOSS_MANTIS_MODE.MOVE_TERRITORY:
                UpdateMove();
                break;
            case BOSS_MANTIS_MODE.DEAD:
                break;
        }
    }

    public override void StartRangeAttack()
    {
        m_canAttack = false;
        m_color = m_renderer.material.color;
        m_rangeRenderere.material.color = m_rangeColor;
        // m_rangeAttack.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        m_rangeAttack.gameObject.SetActive(true);
        // 範囲攻撃用オブジェクトを敵の方向へ調整
        m_rangeAttack.SetRange(m_target);
        m_coroutine = StartCoroutine(ChargeEnemyRangeAttack());
    }

    private IEnumerator ChargeEnemyRangeAttack()
    {
        // 色をだんだん薄くする
        while (true)
        {
            m_nowTime += Time.deltaTime;
            float ratio = m_nowTime / m_rangeAttackTime;
            var color = m_rangeRenderere.material.color;
            color.a = 1.0f - ratio;
            m_rangeRenderere.material.color = color;
            if (ratio > 0.8f)
            {
                color.a = 1.0f;
                m_rangeRenderere.material.color = color;
                m_nowTime = 0.0f;
                // 攻撃コルーチンへ
                m_mode = BOSS_MANTIS_MODE.RANGE_ATTACKA;
                m_coroutine = StartCoroutine(EnemyRangeAttack());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator EnemyRangeAttack()
    {
        // 範囲攻撃の当たり判定を1フレームだけする
        m_rangeAttack.gameObject.layer = m_rangeLayerNo;
        yield return new WaitForSeconds(0.1f);
        m_rangeAttack.gameObject.layer = 12;
        yield return new WaitForSeconds(m_afterAttackTime);
        m_rangeAttack.DeleteList();
        m_mode = BOSS_MANTIS_MODE.MOVE_ATTACK;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_renderer.material.color = m_color;
        Color color = m_rangeRenderere.material.color;
        color.a = 0.0f;
        m_rangeRenderere.material.color = color;
        m_rangeAttack.transform.eulerAngles = Vector3.zero;
        m_rangeAttack.transform.localPosition = Vector3.zero;
        m_attackedTime = Time.time;
        // 死んでいるならターゲットから削除
        if(m_targetType==TARGET_TYPE.MINION)
        {
            if(m_target.GetComponent<Minion>().m_mode==Minion.MINION_MODE.DEAD)
            {
                m_target = null;
            }
        }
        yield break;
    }

    public override void ResetEnemy()
    {
        // 攻撃を中止する
        m_mode = BOSS_MANTIS_MODE.MOVE_TERRITORY;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
        m_nowTime = 0.0f;
        m_renderer.material.color = m_color;
        m_vec = Vector2.zero; Color color = m_rangeRenderere.material.color;
        color.a = 0.0f;
        m_rangeRenderere.material.color = color;
        m_rangeAttack.transform.eulerAngles = Vector3.zero;
        m_rangeAttack.transform.localPosition = Vector3.zero;
        m_rangeAttack.gameObject.layer = 12;
    }

}
