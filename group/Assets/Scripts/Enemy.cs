using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour {
    public enum ENEMY_MODE {
        WAIT,
        MOVE_ATTACK,
        WAIT_ATTACK_COOLDWON,
        WAIT_RANGE_ATTACK_COOLDWON,
        SETUP_ATTACK,
        SETUP_RANGE_ATTACK,
        ATTACK,
        RANGE_ATTACK,
        AFTER_ATTACK,
        MOVE_TERRITORY,
        DEAD,
    }
    public enum ENEMY_TYPE {
        LADYBIRD,
        ANTS,
        BEE,
        BUTTERFLY,
        SPIDER,
        DRAGONFLY,
        MANTIS,
        BOSS_ANTS,
    }

    public enum TARGET_TYPE
    {
        PLAYER,
        MINION,
    }
    public ENEMY_MODE m_mode;
    public ENEMY_TYPE m_type;
    public GameObject m_hpui;
    private MinionController m_minionController = null;
    private EnemyController m_enemyController = null;
    [SerializeField]
    private Rigidbody2D m_rigidbody = null;
    public int m_HP = 3;
    private int m_maxHP = 0;
    [SerializeField]
    private int m_damage = 2;
    public int m_Damage { get { return m_damage; } private set { m_damage = value; } }
    [SerializeField]
    private float m_speed = 4;
    public float m_Speed { get { return m_speed; } private set { m_speed = value; } }
    [SerializeField]
    private float m_mutekitime = 3.0f;
    [SerializeField]
    public bool m_mutekiFlag = false;
    [SerializeField]
    private float m_territoryDistance = 6.0f;
    [SerializeField]
    private float m_stopAttackDistance = 2.0f;
    public GameObject m_target;
    public Minion m_targetMinion;
    private TARGET_TYPE m_targetType;
    public Vector3 m_territoryPos { get; set; }
    private Vector2 m_vec = Vector2.zero;
    private bool m_isArrive = false;
    private Coroutine m_coroutine;

    // 移動関係
    [SerializeField]
    private Vector2 m_moveDirection = Vector2.zero;
    [SerializeField]
    private float m_moveDistance = 3.0f;

    // 攻撃関係
    [SerializeField]
    private EnemyAttackCollider m_enemyAttack = null;
    [SerializeField]
    private SpriteRenderer m_renderer = null;
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
    [SerializeField]
    private bool isRange = true;

    // 範囲攻撃関係
    [SerializeField]
    private EnemyRangeAttack m_rangeAttack = null;
    [SerializeField]
    private float m_addAlphaRange = 0.02f;
    [SerializeField]
    private float m_attackTimeRange = 2.0f;
    [SerializeField]
    private float m_rangeAttackCoolDown=3.0f;
    [SerializeField]
    private float m_rangeAttackTime = 2.0f;
    [SerializeField]
    private float m_rangeAfterAttackTime = 1.0f;
    private Color m_rangeColor;
    [SerializeField]
    private SpriteRenderer m_rangeRenderere = null;
    private int m_rangeLayerNo = -1;

    [SerializeField]
    private int m_levelPoint = 1;


    private void Awake()
    {
        m_minionController = GameObject.Find("MinionController").GetComponent<MinionController>();
        m_enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_mode = ENEMY_MODE.WAIT;
        m_rangeColor = m_rangeRenderere.material.color;
        Color color = m_rangeRenderere.material.color;
        color.a = 0.0f;
        m_rangeRenderere.material.color= color;
        m_rangeLayerNo = m_rangeAttack.gameObject.layer;
        m_rangeAttack.gameObject.layer = 12;
        m_rangeAttack.gameObject.SetActive(false);
        m_territoryPos = gameObject.transform.position;
        m_maxHP = m_HP;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(m_mode);
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
                    if (isRange)
                    {
                        m_mode = ENEMY_MODE.WAIT_RANGE_ATTACK_COOLDWON;
                    }
                    else
                    {
                        m_mode = ENEMY_MODE.WAIT_ATTACK_COOLDWON;

                    }
                }
                else
                {
                    // テリトリーの外へ出たらテリトリーへ戻る
                    if (CheckTerritory() || (m_targetType==TARGET_TYPE.MINION&& m_targetMinion.m_mode != Minion.MINION_MODE.ESCAPE))
                    {
                        // ターゲットの方へ
                        UpdateMoveToTarget();
                    }
                    else
                    {
                        m_mode = ENEMY_MODE.MOVE_TERRITORY;
                    }
                }
                break;
            case ENEMY_MODE.WAIT_ATTACK_COOLDWON:
                if (m_target == null)
                {
                    ResetEnemy();
                    break;
                }
                // 攻撃可能なら攻撃へ
                if (CheckCanAttack())
                {
                    m_mode = ENEMY_MODE.SETUP_ATTACK;
                    StartAttack();
                }
                //if (!CheckTargetLeave())
                //{
                //}
                //else
                //{
                //    ResetEnemy();
                //}
                break;
            case ENEMY_MODE.WAIT_RANGE_ATTACK_COOLDWON:
                // 攻撃可能なら攻撃へ
                if (CheckCanRangeAttack())
                {
                    m_mode = ENEMY_MODE.SETUP_RANGE_ATTACK;
                    StartRangeAttack();
                }
                break;
            case ENEMY_MODE.SETUP_ATTACK:
                if (m_target == null)
                {
                    ResetEnemy();
                }
                break;
            case ENEMY_MODE.SETUP_RANGE_ATTACK:
                break;
            case ENEMY_MODE.ATTACK:
                if (m_target == null)
                {
                    ResetEnemy();
                }
                //if (CheckTargetLeave())
                //{
                //    ResetEnemy();
                //}
                break;
            case ENEMY_MODE.RANGE_ATTACK:

                break;
            case ENEMY_MODE.MOVE_TERRITORY:
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
                    if (isRange)
                    {
                        m_mode = ENEMY_MODE.WAIT_RANGE_ATTACK_COOLDWON;
                    }
                    else
                    {
                        m_mode = ENEMY_MODE.WAIT_ATTACK_COOLDWON;

                    }
                }
                else
                {
                    // テリトリーに侵入したら迎撃開始
                    if (FindTarget())
                    {
                        m_mode = ENEMY_MODE.MOVE_ATTACK;
                    }
                    else
                    {
                        if ((transform.position - m_territoryPos).magnitude <= 0.1f)
                        {
                            transform.position = m_territoryPos;
                            m_mode = ENEMY_MODE.WAIT;
                            m_rigidbody.velocity = Vector2.zero;
                        }
                        UpdateMoveToTerritory();
                    }
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
        m_vec = m_target.transform.position - transform.position;
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
        GameObject target = null;
        float minDistance = 99999.0f;
        // すべてのミニオンが射程内にいるかチェック
        foreach(var minon in m_minionController.m_Minions)
        {
            if (minon.m_mode == Minion.MINION_MODE.DEAD) continue;
            if ((minon.transform.position - m_territoryPos).magnitude <= m_territoryDistance&&
                    (minon.transform.position - gameObject.transform.position).magnitude<minDistance)
            {
                target = minon.gameObject;
                minDistance = (minon.transform.position - gameObject.transform.position).magnitude;
            }
        }
        if ((m_minionController.m_Player.transform.position - m_territoryPos).magnitude <= m_territoryDistance &&
        (m_minionController.m_Player.transform.position - gameObject.transform.position).magnitude < minDistance)
        {
            target = m_minionController.m_Player;
        }

        if (target == null)
        {
            m_target = target;
            m_targetMinion = null;
            return false;
        }
        else
        {
            m_target=target;
            if (m_target.tag == "Minion")
            {
                m_targetType=TARGET_TYPE.MINION;
                m_targetMinion=m_target.GetComponent<Minion>();
            }
            else
            {
                m_targetType = TARGET_TYPE.PLAYER;
            }
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
        if(m_target==null)
        {
            return true;
        }
        if((transform.position-m_target.transform.position).magnitude>=m_stopAttackDistance)
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

    public void RevivalEnemy()
    {
        m_HP = m_maxHP;
        transform.position = m_territoryPos;
        ResetEnemy();
    }

    public void StartAttack()
    {
        m_canAttack = false;
        m_color = m_renderer.material.color;
        m_coroutine = StartCoroutine(ChargeEnemyAttack());
    }

    public void StartRangeAttack()
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
                m_mode = ENEMY_MODE.MOVE_ATTACK;
                m_coroutine = StartCoroutine(EnemyAttack());
                yield break;
            }
            yield return null;
        }
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
                m_mode = ENEMY_MODE.RANGE_ATTACK;
                m_coroutine = StartCoroutine(EnemyRangeAttack());
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
        if (m_targetType == TARGET_TYPE.MINION)
        {
            m_targetMinion.Damage(m_damage, this);
            if (m_targetMinion.m_mode == Minion.MINION_MODE.DEAD)
            {
                m_enemyAttack.m_HitObjList.Remove(m_target);
                m_target.gameObject.SetActive(false);
            }
        }
        else
        {
            m_target.GetComponent<Player>().Damage(m_damage);
        }
        yield return new WaitForSeconds(m_afterAttackTime);
        m_mode = ENEMY_MODE.MOVE_ATTACK;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_renderer.material.color = m_color;
        m_attackedTime = Time.time;
        m_target = null;
        yield break;
    }

    IEnumerator EnemyRangeAttack()
    {
        // 範囲攻撃の当たり判定を1フレームだけする
        //var collider = m_rangeAttack.gameObject.GetComponent<BoxCollider2D>();
        //collider.enabled = true;
        m_rangeAttack.gameObject.layer = m_rangeLayerNo;
        yield return new WaitForSeconds(0.1f);
        //yield return null;
        //yield return null;
        m_rangeAttack.gameObject.layer = 12;
        //collider.enabled = false;
        // Debug.Log(m_rangeAttack.m_HitMinionList.Count + "人に攻撃");
        //foreach (var hitObj in m_rangeAttack.m_HitObjList)
        //{
        //    if (hitObj.tag == "Minion")
        //    {
        //        hitObj.GetComponent<Minion>().Damage(m_rangeAttackDamage, this);
        //    }
        //    else
        //    {
        //        hitObj.GetComponent<Player>().Damage(m_rangeAttackDamage);
        //    }
        //}
        //List<GameObject> objs = new List<GameObject>(m_rangeAttack.m_HitObjList);
        //foreach (var obj in objs)
        //{
        //    if (obj.tag == "Minion")
        //    {
        //        if (obj.GetComponent<Minion>().m_mode == Minion.MINION_MODE.DEAD)
        //        {
        //            m_rangeAttack.m_HitObjList.Remove(obj);
        //            obj.SetActive(false);
        //        }
        //    }
        //}
        yield return new WaitForSeconds(m_afterAttackTime);
        m_rangeAttack.DeleteList();
        m_mode = ENEMY_MODE.MOVE_ATTACK;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_renderer.material.color = m_color;
        Color color = m_rangeRenderere.material.color;
        color.a = 0.0f;
        m_rangeRenderere.material.color = color;
        m_rangeAttack.transform.eulerAngles = Vector3.zero;
        m_rangeAttack.transform.localPosition = Vector3.zero;

        m_attackedTime = Time.time;
        m_target = null;
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

    public bool CheckCanRangeAttack()
    {
        if (m_canAttack)
        {
            return true;
        }
        else
        {
            if (Time.time - m_attackedTime >= m_rangeAttackCoolDown)
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
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
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
            if (m_type == ENEMY_TYPE.BOSS_ANTS)
            {
                m_enemyController.StartCoroutine(m_enemyController.ChangeScene());
            }
            m_minionController.AddExperiencePoint(m_levelPoint);
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
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }

    }

}

