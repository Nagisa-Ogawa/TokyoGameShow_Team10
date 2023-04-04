using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Enemy;

public class Minion:MonoBehaviour
{
    public enum MINION_MODE
    {
        FOLLOW,
        MOVE_ENEMY,
        ATTACK,
        ESCAPE,
        WAIT,
        DAMAGE,
        DEAD,
    }
    public enum MINION_TYPE
    {
        MINION_A,
        MINION_B,
    }
    public GameObject m_playerBack {get;private set;}
    public MINION_MODE m_mode;
    public MINION_TYPE m_type;
    public int m_HP = 3;
    public int m_maxHP {get; private set;}
    public int m_damage = 2;
    public Renderer m_renderer = null;
    public float m_addAlpha = 0.02f;
    public float m_attackTime = 2.0f;
    public float m_nowTime = 0.0f;
    public float m_afterAttackTime = 1.0f;
    public float m_attackCoolDown = 3.0f;
    public float m_attackedTime = 0.0f;
    public Color m_color;
    public bool m_canAttack = true;
    public float m_escapeDistance = 50.0f;
    public float m_speed = 5.0f;
    [SerializeField]
    private float m_stopAttackDistance = 2.0f;
    [SerializeField]
    private float m_stopAttackBossDistance = 5.0f;

    public Param m_Param = null;
    public Rigidbody2D m_rigidbody { get; set; }
    private MinionController m_minionController = null;
    public MinionController m_MinionController { get { return m_minionController; } protected set { m_minionController = value; } }
    public GameObject m_player { get; private set; }
    private List<Minion> m_neighborsList = new List<Minion>();
    public List<Minion> m_NeighborsList { get { return m_neighborsList; } private set { m_neighborsList = value; } }
    public Vector2 m_pos { get; private set; }
    private Vector2 m_vec = Vector3.zero;
    private Vector2 m_beforPos = Vector2.zero;
    public GameObject m_target { get; set; }
    public Enemy m_targetEnemy { get; set; }

    public GameObject m_hpui { get; set; }

    private void Awake()
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
        // m_vec = transform.up * Random.Range(m_Param.minSpeed, m_Param.maxSpeed);
        m_maxHP = m_HP;
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
                UpdateMoveToTarget();
                break;
            case MINION_MODE.ATTACK:
                if (CheckTargetLeave())
                {
                    StopAttack();
                }
                break;
            case MINION_MODE.ESCAPE:
                if (m_targetEnemy == null) return;
                if((gameObject.transform.position-m_targetEnemy.transform.position).magnitude >m_escapeDistance)
                {
                    m_mode = MINION_MODE.WAIT;
                    //var rot = Quaternion.FromToRotation(Vector3.up, m_target.transform.position - transform.position);
                    //transform.rotation = rot;
                    m_rigidbody.bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    UpdateEscapeToTarget();
                }
                break;
            case MINION_MODE.WAIT:
                break;
            case MINION_MODE.DAMAGE:
                break;
            case MINION_MODE.DEAD:
                break;
        }
    }

    public void FixedUpdate()
    {
        switch (m_mode)
        {
            case MINION_MODE.FOLLOW:
                UpdateMove();
                break;
            case MINION_MODE.MOVE_ENEMY:
                UpdateMove();
                break;
            case MINION_MODE.ATTACK:
                break;
            case MINION_MODE.ESCAPE:
                UpdateMove();
                break;
            case MINION_MODE.WAIT:
                break;
            case MINION_MODE.DAMAGE:
                break;
            case MINION_MODE.DEAD:
                break;
            default:
                break;
        }
    }

    public void UpdateFindNeighbors()
    {
        m_neighborsList.Clear();

        if (!m_minionController) return;

        var prodThresh = Mathf.Cos(m_Param.neighborFOV * Mathf.Deg2Rad);
        var distThresh = m_Param.neighborDistance;

        foreach (var minion in m_minionController.m_Minions)
        {
            if (minion == this) continue;

            var to = minion.m_pos - m_pos;
            var dist = to.magnitude;
            // 視界内ならリストへ格納
            if (dist < distThresh)
            {
                var dir = to.normalized;
                var fwd = m_vec.normalized;
                var prod = Vector3.Dot(fwd, dir);
                if (prod > prodThresh)
                {
                    m_neighborsList.Add(minion);
                }
            }
        }
    }

    public void UpdateBinding()
    {
        if (m_neighborsList.Count == 0) return;

        var averagePos = Vector3.zero;
        foreach (var minion in m_minionController.m_Minions)
        {
            averagePos += minion.transform.position;
        }
        averagePos /= (m_minionController.m_Minions.Count + 1);
        averagePos += m_playerBack.transform.position;
        averagePos /= 2;
        m_vec += new Vector2((averagePos.x - m_pos.x), (averagePos.y - m_pos.y)) * m_Param.bindingWeight;

    }

    public void UpdateSeparation()
    {
        if (m_neighborsList.Count == 0) return;

        Vector2 vec = Vector2.zero;
        foreach (var neighbor in m_neighborsList)
        {
            vec += (m_pos - neighbor.m_pos).normalized;
        }
        //vec += (m_pos - m_player.transform.position).normalized;
        //vec /= m_neighborsList.Count+1;
        vec /= (m_neighborsList.Count + 1);

        m_vec += vec * m_Param.separationWeight;

    }

    public void UpdateAlignment()
    {
        if (m_neighborsList.Count == 0) return;

        Vector2 averageVel = Vector2.zero;
        foreach (var neighbor in m_neighborsList)
        {
            averageVel += neighbor.m_vec;
        }
        averageVel /= (m_neighborsList.Count + 1);
        m_vec += (averageVel - m_vec) * m_Param.alignmentWeight;
        // Debug.Log(averageVel.magnitude);
    }

    public void UpdateMoveToTarget()
    {
        if (m_target == null)
        {
            Debug.Log("ターゲットがいません");
            return;
        }
        m_vec = m_target.transform.position - transform.position;
    }

    public void UpdateEscapeToTarget()
    {
        m_vec = (m_target.transform.position - transform.position) * -1.0f;
    }
    public void UpdateMove()
    {
        // float dt = Time.deltaTime;
        Vector2 m_velocity = Vector2.zero;
        m_velocity = m_vec;
        Vector3 dir = m_velocity.normalized;
        // float speed = Random.Range(m_Param.minSpeed, m_Param.maxSpeed);
        m_velocity = m_speed * dir;
        //var rot = Quaternion.FromToRotation(Vector3.up, m_velocity);
        //transform.rotation = rot;
        m_rigidbody.velocity = m_velocity;
        m_vec = Vector2.zero;
        // m_velocity = Vector2.zero;
    }

    public bool CheckTargetLeave()
    {
        if (m_targetEnemy == null)
        {
            return true;
        }
        if(m_targetEnemy.m_type==ENEMY_TYPE.ENEMY_A||m_targetEnemy.m_type==ENEMY_TYPE.ENEMY_B)
        {
            if ((transform.position - m_targetEnemy.transform.position).magnitude >= m_stopAttackDistance)
            {
                return true;
            }
        }
        else if(m_targetEnemy.m_type==ENEMY_TYPE.BOSS_B)
        {
            if ((transform.position - m_targetEnemy.transform.position).magnitude >= m_stopAttackBossDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void StartAttack()
    {
        m_canAttack = false;
        StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack() 
    { 
        // 色をだんだん薄くする
        while (true)
        {
            m_nowTime += Time.deltaTime;
            float ratio = m_nowTime / m_attackTime;
            Debug.Log(ratio);
            var color = m_renderer.material.color;
            color.a = 1.0f - ratio;
            m_renderer.material.color = color;
            if (ratio > 0.8f)
            {
                color.a = 1.0f;
                m_renderer.material.color = color;
                m_nowTime = 0.0f;
                // 攻撃コルーチンへ
                StartCoroutine(Attack());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator Attack()
    {
        //var color=m_renderer.material.color;
        //color = Color.black;
        //m_renderer.material.color = color;
        // 敵のHPを減らす
        m_targetEnemy.Damage(m_damage);
        yield return new WaitForSeconds(m_afterAttackTime);
        m_mode = m_minionController.m_mode;
        m_renderer.material.color = m_color;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_attackedTime = Time.time;
        yield break;
    }

    public void Damage(int damage)
    {
        m_HP-=damage;
        if (m_HP > 0)
        {
            // 被弾時処理
        }
        else
        {
            gameObject.SetActive(false);
            m_hpui.SetActive(false);
            // ターゲットを変更
            m_mode = MINION_MODE.DEAD;
            m_renderer.material.color = m_color;
            m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
            StopAllCoroutines();
            m_target = null;
            m_targetEnemy = null;
            m_nowTime = 0.0f;
        }
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

    public void StopAttack()
    {
        m_mode = MINION_MODE.MOVE_ENEMY;
        m_renderer.material.color = m_color;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        StopAllCoroutines();
        m_nowTime = 0.0f;
    }

    public void StopMinionCoroutine()
    {
        StopAllCoroutines();
    }

}
