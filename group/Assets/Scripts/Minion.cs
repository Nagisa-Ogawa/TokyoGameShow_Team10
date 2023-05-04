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
        WAIT_COOLDOWN,
        ATTACK,
        ESCAPE,
        WAIT,
        DAMAGE,
        DEAD,
    }
    public enum MINION_TYPE
    {
        LADYBIRD,
        ANTS,
        BEE,
        BUTTERFLY,
        SPIDER,
        DRAGONFLY,
        MANTIS,
    }
    public GameObject m_playerBack {get;private set;}
    public MINION_MODE m_mode;
    public MINION_TYPE m_type;
    public int m_HP = 3;
    public int m_maxHP {get; private set;}
    public int m_damage = 2;
    public int m_Damage { get { return m_damage; } private set { m_damage = value; } }
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
    public float m_Speed { get { return m_speed; } private set { m_speed = value; } }
    [SerializeField]
    private float m_escapeSpeed = 10.0f;
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

    private Coroutine m_coroutine = null;
    [SerializeField]
    private AttackCollider m_attackCollider = null;

    // レベルアップ関係
    [SerializeField]
    private int m_level = 1;
    public int m_Level { get { return m_level; } private set {  m_level = value; } }
    [SerializeField]
    private int m_addHp = 5;
    public int m_AddHp { get {  return m_addHp; } private set {  m_addHp = value; } }
    [SerializeField]
    private int m_addDamage = 1;
    public int m_AddDamage { get {  return m_addDamage; } private set { m_addDamage = value; } }
    [SerializeField]
    private int m_addSpeed = 1;
    public int m_AddSpeed { get {  return m_addSpeed; } private set { m_addSpeed = value; } }


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
                if(m_attackCollider.m_HitEnemyList.Count > 0)
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
                if ((gameObject.transform.position-m_targetEnemy.transform.position).magnitude >m_escapeDistance)
                {
                    m_mode = MINION_MODE.WAIT;
                    //var rot = Quaternion.FromToRotation(Vector3.up, m_target.transform.position - transform.position);
                    //transform.rotation = rot;
                    m_rigidbody.velocity= Vector3.zero;
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
        if (m_mode == MINION_MODE.ESCAPE)
        {
            m_velocity = m_escapeSpeed * dir;
        }
        else
        {
            m_velocity = m_speed * dir;
        }
        //var rot = Quaternion.FromToRotation(Vector3.up, m_velocity);
        //transform.rotation = rot;
        m_rigidbody.velocity = m_velocity;
        m_vec = Vector2.zero;
        // m_velocity = Vector2.zero;
    }


    public void CheckReturnPlayer()
    {
        if (m_mode == MINION_MODE.DEAD)
        {
            return;
        }
        Vector3 pPos = m_player.transform.position;
        var player = m_player.GetComponent<Player>();
        if ((pPos - transform.position).magnitude >= player.m_ReturnDistance)
        {
            m_minionController.ChangeMode(MINION_MODE.FOLLOW);
        }
    }
    public void StartAttack()
    {
        m_mode = Minion.MINION_MODE.ATTACK;
        m_canAttack = false;
        m_coroutine = StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack() 
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
                m_coroutine = StartCoroutine(Attack());
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

    public void Damage(int damage,Enemy enemy)
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
            // 敵の攻撃範囲内で死亡したなら敵の攻撃可能リストから自分を消去
            // enemy.DeleteMinionAttackList(this);
            // ターゲットを変更
            m_mode = MINION_MODE.DEAD;
            m_renderer.material.color = m_color;
            m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
            }
            m_target = null;
            m_targetEnemy = null;
            m_nowTime = 0.0f;
            // 全滅かどうか判定
            m_minionController.StartCoroutine(m_minionController.CheckAllDied());
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

    public void StopMinionCoroutine()
    {
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
    }

    public void LevelUp()
    {
        Debug.Log("レベルアップ");
        m_level++;
        m_maxHP += m_addHp;
        m_damage += m_addDamage;
        m_speed += m_addSpeed;
        // m_chaseSpeed += m_addSpeed;
    }

}
