using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour {
    public enum ENEMY_MODE
    {
        WAIT,
        MOVE_ATTACK,
        SETUP_ATTACK,
        ATTACK,
        MOVE_TERRITORY,
        DEAD,
    }
    public enum ENEMY_TYPE
    {
        ENEMY_A,
        ENEMY_B,
        BOSS_B,
    }
    public ENEMY_MODE m_mode;
    public ENEMY_TYPE m_type;
    public GameObject m_hpui;
    private MinionController m_minionController = null;
    private EnemyController m_enemyController= null;
    [SerializeField]
    private Rigidbody2D m_rigidbody = null;
    public int m_HP = 3;
    [SerializeField]
    private int m_damage = 2;
    [SerializeField]
    private float m_speed = 4;
    [SerializeField]
    private float m_mutekitime = 3.0f;
    [SerializeField]
    public bool m_mutekiFlag = false;
    [SerializeField]
    private float m_territoryDistance = 6.0f;
    [SerializeField]
    private float m_stopAttackDistance = 2.0f;
    public Minion m_targetMinion;
    public Vector3 m_territoryPos { get; set; }
    private Vector2 m_vec = Vector2.zero;
    private bool m_isArrive = false;

    // 移動関係
    [SerializeField]
    private Vector2 m_moveDirection = Vector2.zero;
    [SerializeField]
    private float m_moveDistance = 3.0f;
    [SerializeField]
    private float m_moveSpeed = 3.0f;

    // 攻撃関係
    [SerializeField]
    private Renderer m_renderer = null;
    [SerializeField]
    private float m_addAlpha = 0.02f;
    [SerializeField]
    private float m_chaseSpeed = 4.0f;
    public float m_attackTime = 2.0f;
    public float m_nowTime = 0.0f;
    public float m_afterAttackTime = 1.0f;
    public float m_attackCoolDown = 3.0f;
    public float m_attackedTime = 0.0f;
    public Color m_color;
    public bool m_canAttack = true;


    private void Awake()
    {
        m_minionController = GameObject.Find("MinionController").GetComponent<MinionController>();
        m_enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_mode = ENEMY_MODE.WAIT;
        m_territoryPos = gameObject.transform.position;
        m_color=m_renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_mode)
        {
            case ENEMY_MODE.WAIT:
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
                        m_mode = ENEMY_MODE.MOVE_ATTACK;
                    }
                }
                else
                {
                    m_mode = ENEMY_MODE.MOVE_TERRITORY;
                }
                break;
            case ENEMY_MODE.MOVE_ATTACK:
                if (m_targetMinion == null)
                {
                    ResetEnemy();
                }
                // テリトリーの外へ出たらテリトリーへ戻る
                if (CheckTerritory()||m_targetMinion.m_mode!=Minion.MINION_MODE.ESCAPE)
                {
                    // ターゲットの方へ
                    UpdateMoveToTarget();
                }
                else
                {
                    m_mode = ENEMY_MODE.MOVE_TERRITORY;
                }
                break;
            case ENEMY_MODE.SETUP_ATTACK:
                if (m_targetMinion == null)
                {
                    ResetEnemy();
                }
                if (!CheckTargetLeave())
                {
                    // 攻撃可能なら攻撃へ
                    if (CheckCanAttack())
                    {
                        m_mode = ENEMY_MODE.ATTACK;
                        StartAttack();
                    }
                }
                else
                {
                    ResetEnemy();
                }
                break;
            case ENEMY_MODE.ATTACK:
                if (m_targetMinion == null)
                {
                    ResetEnemy();
                }
                if (CheckTargetLeave())
                {
                    ResetEnemy();
                }
                break;
            case ENEMY_MODE.MOVE_TERRITORY:
                // テリトリーに侵入したら迎撃開始
                if(FindTarget())
                {
                    m_mode = ENEMY_MODE.MOVE_ATTACK;
                }
                else
                {
                    if ((transform.position - m_territoryPos).magnitude<=0.1f)
                    {
                        transform.position = m_territoryPos;
                        m_mode = ENEMY_MODE.WAIT;
                        m_rigidbody.velocity = Vector2.zero;
                    }
                    UpdateMoveToTerritory();
                }
                break;
            case ENEMY_MODE.DEAD:
                break;
        }
    }



    private void FixedUpdate()
    {
        switch (m_mode)
        {
            case ENEMY_MODE.WAIT:
                //var rot = Quaternion.FromToRotation(Vector3.up, m_moveDirection);
                //transform.rotation = rot;
                m_rigidbody.velocity = m_moveDirection;
                break;
            case ENEMY_MODE.MOVE_ATTACK:
                UpdateMove();
                break;
            case ENEMY_MODE.ATTACK:
                break;
            case ENEMY_MODE.MOVE_TERRITORY:
                UpdateMove();
                break;
            case ENEMY_MODE.DEAD:
                break;
        }
    }

    public void UpdateMoveToTarget()
    {
        m_vec = m_targetMinion.transform.position - transform.position;
    }

    public void UpdateMoveToTerritory()
    {
        m_vec = m_territoryPos - transform.position;
    }

    public void UpdateMove()
    {
        if(m_vec==Vector2.zero) return;
        Vector2 m_velocity = Vector2.zero;
        m_velocity = m_vec;
        Vector3 dir = m_velocity.normalized;
        if (m_mode == ENEMY_MODE.MOVE_TERRITORY)
        {
            if ((transform.position - m_territoryPos).magnitude <= m_speed)
            {
                m_velocity = (transform.position - m_territoryPos).magnitude * dir;
            }
            else
            {
                m_velocity = m_speed * dir;
            }
            Debug.Log(m_velocity);
        }
        else
        {
            m_velocity = m_chaseSpeed * dir;
        }
        //var rot = Quaternion.FromToRotation(Vector3.up, m_velocity);
        //transform.rotation = rot;
        m_rigidbody.velocity = m_velocity;
        // m_rigidbody.MovePosition(transform.position+(Vector3)m_velocity*Time.deltaTime);
        m_vec = Vector2.zero;
    }

    public bool FindTarget()
    {
        Minion target = null;
        float minDistance = 99999.0f;
        // すべてのミニオンが射程内にいるかチェック
        foreach(var minon in m_minionController.m_Minions)
        {
            if (minon.m_mode == Minion.MINION_MODE.DEAD) continue;
            if ((minon.transform.position - m_territoryPos).magnitude <= m_territoryDistance&&
                    (minon.transform.position - gameObject.transform.position).magnitude<minDistance)
            {
                target = minon;
                minDistance = (minon.transform.position - gameObject.transform.position).magnitude;
            }
        }
        if(target == null)
        {
            m_targetMinion = target;
            return false;
        }
        else
        {
            m_targetMinion=target;
            return true;
        }
    }

    public bool CheckTerritory()
    {
        if((m_territoryPos-gameObject.transform.position).magnitude<m_territoryDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckTargetLeave()
    {
        if(m_targetMinion==null)
        {
            return true;
        }
        if((transform.position-m_targetMinion.transform.position).magnitude>=m_stopAttackDistance)
        {
            return true;
        }
        return false;
    }

    public bool CheckFolding()
    {
        if ((m_territoryPos - transform.position).magnitude > m_moveDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetEnemy()
    {
        // 攻撃を中止する
        m_mode = ENEMY_MODE.MOVE_TERRITORY;
        StopAllCoroutines();
        m_nowTime = 0.0f;
        m_renderer.material.color = m_color;
        m_vec = Vector2.zero;
    }

    public void StartAttack()
    {
        m_canAttack = false;
        StartCoroutine(ChargeEnemyAttack());
    }

    private IEnumerator ChargeEnemyAttack()
    {
        // 色をだんだん薄くする
        while (true)
        {
            m_nowTime += Time.deltaTime;
            float ratio = m_nowTime / m_attackTime;
            var color = m_renderer.material.color;
            color.a = 1.0f - ratio;
            m_renderer.material.color = color;
            if (ratio > 0.8f)
            {
                color.a = 1.0f;
                m_renderer.material.color = color;
                m_nowTime = 0.0f;
                // 攻撃コルーチンへ
                StartCoroutine(EnemyAttack());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator EnemyAttack()
    {
        //var color = m_renderer.material.color;
        //color = Color.black;
        //m_renderer.material.color = color;
        // 敵のHPを減らす
        m_targetMinion.Damage(m_damage);
        yield return new WaitForSeconds(m_afterAttackTime);
        m_mode = ENEMY_MODE.MOVE_TERRITORY;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_renderer.material.color = m_color;
        m_attackedTime = Time.time;
        m_targetMinion = null;
        yield break;
    }

    public bool CheckCanAttack()
    {
        if (m_canAttack)
        {
            return true;
        }
        else
        {
            if (Time.time - m_attackedTime >= m_attackCoolDown)
            {
                m_canAttack = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void StopEnemyCoroutine()
    {
        StopAllCoroutines();
    }



    public void Damage(int damage)
    {
        m_HP -= damage;
        if(m_HP>0)
        {
            // 被弾時処理
        }
        else
        {
            if (m_type == ENEMY_TYPE.BOSS_B)
            {
                m_enemyController.StartCoroutine(m_enemyController.ChangeScene());
            }
            gameObject.SetActive(false);
            m_hpui.SetActive(false);
            // ターゲットを変更
            m_mode = ENEMY_MODE.DEAD;
            m_minionController.ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);
            m_enemyController.CheckBecomeMinion(this);
        }
    }

    public void StopMinionCoroutine()
    {
        StopAllCoroutines();
    }



}

