using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(Enemy))]
#endif

public class Boss_Drogonfly : Enemy
{
    public enum BOSS_DROGONFLY_MODE {
        WAIT,
        MOVE_ATTACK,
        WAIT_RANGE_ATTACK_COOLDWON,
        CREATE_ENEMY,
        SETUP_RANGE_ATTACK,
        RANGE_ATTACK,
        AFTER_ATTACK,
        MOVE_TERRITORY,
        DEAD,
    }


    public BOSS_DROGONFLY_MODE m_mantisMode { get; private set; }

    // 範囲攻撃関係
    [SerializeField]
    protected EnemyRangeAttack m_rangeAttackB = null;
    protected Color m_rangeColorB;
    [SerializeField]
    protected SpriteRenderer m_rangeRenderereB = null;
    int m_choice = 0;

    // 敵作成関係
    [SerializeField]
    private int m_createEnemyNum = 4;
    private List<GameObject> m_enemies = new List<GameObject>();

    private void Awake()
    {
        m_minionController = GameObject.Find("MinionController").GetComponent<MinionController>();
        m_enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_mantisMode = BOSS_DROGONFLY_MODE.WAIT;
        m_type = ENEMY_TYPE.BOSS_MANTIS;
        m_rangeColor = m_rangeRenderere.color;
        m_rangeColorB = m_rangeRenderereB.color;
        m_color = m_renderer.color;
        Color color = m_rangeRenderere.color;
        color.a = 0.0f;
        m_rangeRenderere.color = color;
        color = m_rangeRenderereB.color;
        color.a = 0.0f;
        m_rangeRenderereB.color = color;
        m_rangeLayerNo = m_rangeAttack.gameObject.layer;
        m_rangeAttack.gameObject.layer = 12;
        m_rangeAttack.gameObject.SetActive(false);
        m_rangeAttackB.gameObject.layer = 12;
        m_rangeAttackB.gameObject.SetActive(false);
        m_territoryPos = gameObject.transform.position;
        m_maxHP = m_HP;
        // 敵を事前に作成
        var enemies = m_enemyController.CreateEnemyBoss(5, m_createEnemyNum);
        foreach (var enemy in enemies)
        {
            m_enemies.Add(enemy);
            enemy.gameObject.SetActive(false);
            enemy.GetComponent<Enemy>().m_hpui.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_mantisMode)
        {
            case BOSS_DROGONFLY_MODE.WAIT:
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
                        m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_ATTACK;
                    }
                }
                else
                {
                    m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_TERRITORY;
                }
                break;
            case BOSS_DROGONFLY_MODE.MOVE_ATTACK:
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
                    m_choice = Random.Range(0, 3);
                    if (m_choice == 2 && CheckDeadAllEnemy() == false)
                    {
                        m_choice = Random.Range(0, 2);
                    }
                    switch (m_choice)
                    {
                        case 0:
                            m_mantisMode = BOSS_DROGONFLY_MODE.WAIT_RANGE_ATTACK_COOLDWON;
                            break;
                        case 1:
                            m_mantisMode = BOSS_DROGONFLY_MODE.WAIT_RANGE_ATTACK_COOLDWON;
                            break;
                        case 2:
                            m_mantisMode = BOSS_DROGONFLY_MODE.CREATE_ENEMY;
                            break;
                    }
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
                        m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_TERRITORY;
                    }
                }
                break;
            case BOSS_DROGONFLY_MODE.WAIT_RANGE_ATTACK_COOLDWON:
                // 攻撃可能なら攻撃へ
                if (CheckCanRangeAttack())
                {
                    m_mantisMode = BOSS_DROGONFLY_MODE.SETUP_RANGE_ATTACK;
                    if (m_choice == 0)
                    {
                        StartRangeAttack(m_rangeAttack, m_rangeRenderere, m_rangeColor);
                    }
                    else
                    {
                        StartRangeAttack(m_rangeAttackB, m_rangeRenderereB, m_rangeColorB);
                    }
                }
                break;
            case BOSS_DROGONFLY_MODE.SETUP_RANGE_ATTACK:
                break;
            case BOSS_DROGONFLY_MODE.RANGE_ATTACK:
                break;
            case BOSS_DROGONFLY_MODE.MOVE_TERRITORY:
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
                    m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_ATTACK;
                }
                else
                {
                    // テリトリーに侵入したら迎撃開始
                    if (FindTarget())
                    {
                        m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_ATTACK;
                    }
                    else
                    {
                        if ((transform.position - m_territoryPos).magnitude <= 0.1f)
                        {
                            transform.position = m_territoryPos;
                            m_mantisMode = BOSS_DROGONFLY_MODE.WAIT;
                            m_rigidbody.velocity = Vector2.zero;
                        }
                        UpdateMoveToTerritory();
                    }
                }
                break;
            case BOSS_DROGONFLY_MODE.CREATE_ENEMY:
                List<Vector3> poss = new List<Vector3>();
                float offset = 3.0f;
                poss.Add(transform.position + new Vector3(offset, offset, 0));
                poss.Add(transform.position + new Vector3(-offset, offset, 0));
                poss.Add(transform.position + new Vector3(offset, -offset, 0));
                poss.Add(transform.position + new Vector3(-offset, -offset, 0));
                // 敵を生成
                for (int i = 0; i < m_createEnemyNum; i++)
                {
                    m_enemies[i].transform.position = poss[i];
                    m_enemies[i].GetComponent<Enemy>().m_territoryPos = poss[i];
                    m_enemies[i].GetComponent<Enemy>().RevivalEnemy();
                    m_enemies[i].SetActive(true);
                    m_enemies[i].GetComponent<Enemy>().m_hpui.gameObject.SetActive(true);
                }
                m_coroutine = StartCoroutine(Wait());
                break;
            case BOSS_DROGONFLY_MODE.AFTER_ATTACK:
                break;
            case BOSS_DROGONFLY_MODE.DEAD:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (m_mantisMode)
        {
            case BOSS_DROGONFLY_MODE.WAIT:
                if (m_moveDirection.x > 0.0f && m_renderer.flipX == false) m_renderer.flipX = true;
                if (m_moveDirection.x < 0.0f && m_renderer.flipX == true) m_renderer.flipX = false;
                m_rigidbody.velocity = m_moveDirection;
                break;
            case BOSS_DROGONFLY_MODE.MOVE_ATTACK:
                UpdateMove();
                break;
            case BOSS_DROGONFLY_MODE.MOVE_TERRITORY:
                UpdateMove();
                break;
            case BOSS_DROGONFLY_MODE.DEAD:
                break;
        }
    }

    public override void UpdateMove()
    {
        if (m_vec == Vector2.zero) return;
        Vector2 m_velocity = Vector2.zero;
        m_velocity = m_vec;
        Vector3 dir = m_velocity.normalized;
        if (m_mantisMode == BOSS_DROGONFLY_MODE.MOVE_TERRITORY)
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
        if (m_velocity.x > 0.0f && m_renderer.flipX == false) m_renderer.flipX = true;
        if (m_velocity.x < 0.0f && m_renderer.flipX == true) m_renderer.flipX = false;
        m_rigidbody.velocity = m_velocity;
        // m_rigidbody.MovePosition(transform.position+(Vector3)m_velocity*Time.deltaTime);
        m_vec = Vector2.zero;
    }


    public void StartRangeAttack(EnemyRangeAttack rangeAttack, SpriteRenderer renderer, Color color)
    {
        m_canAttack = false;
        renderer.color = color;
        // m_rangeAttack.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rangeAttack.gameObject.SetActive(true);
        // 範囲攻撃用オブジェクトを敵の方向へ調整
        rangeAttack.SetRange(m_target);
        m_coroutine = StartCoroutine(ChargeEnemyRangeAttack(rangeAttack, renderer));
    }

    private IEnumerator ChargeEnemyRangeAttack(EnemyRangeAttack rangeAttack, SpriteRenderer renderer)
    {
        // 色をだんだん薄くする
        while (true)
        {
            m_nowTime += Time.deltaTime;
            float ratio = m_nowTime / m_rangeAttackTime;
            var color = renderer.material.color;
            color.a = 1.0f - ratio;
            renderer.material.color = color;
            if (ratio > 0.8f)
            {
                color.a = 1.0f;
                renderer.material.color = color;
                m_nowTime = 0.0f;
                // 攻撃コルーチンへ
                m_mantisMode = BOSS_DROGONFLY_MODE.RANGE_ATTACK;
                m_coroutine = StartCoroutine(EnemyRangeAttack(rangeAttack, renderer));
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator EnemyRangeAttack(EnemyRangeAttack rangeAttack, SpriteRenderer renderer)
    {
        // 範囲攻撃の当たり判定を1フレームだけする
        rangeAttack.gameObject.layer = m_rangeLayerNo;
        yield return new WaitForSeconds(0.1f);
        rangeAttack.gameObject.layer = 12;
        yield return new WaitForSeconds(m_afterAttackTime);
        rangeAttack.DeleteList();
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_renderer.material.color = m_color;
        Color color = renderer.material.color;
        color.a = 0.0f;
        renderer.material.color = color;
        rangeAttack.transform.eulerAngles = Vector3.zero;
        rangeAttack.transform.localPosition = Vector3.zero;
        m_attackedTime = Time.time;
        m_target = null;
        if (FindTarget())
        {
            m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_ATTACK;
        }
        else
        {
            m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_TERRITORY;
        }
        yield break;
    }

    IEnumerator Wait()
    {
        m_mantisMode = BOSS_DROGONFLY_MODE.AFTER_ATTACK;
        yield return new WaitForSeconds(10.0f);
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_target = null;
        if (FindTarget())
        {
            m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_ATTACK;
        }
        else
        {
            m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_TERRITORY;
        }
    }

    private bool CheckDeadAllEnemy()
    {
        foreach (var enemy in m_enemies)
        {
            if (enemy.activeSelf == true)
            {
                return false;
            }
        }
        return true;
    }

    public override void RevivalEnemy()
    {
        m_HP = m_maxHP;
        transform.position = m_territoryPos;
        ResetEnemy();
    }

    public override void ResetEnemy()
    {
        // 攻撃を中止する
        m_mantisMode = BOSS_DROGONFLY_MODE.MOVE_TERRITORY;
        m_mode = ENEMY_MODE.WAIT;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
        m_nowTime = 0.0f;
        m_renderer.material.color = m_color;
        m_vec = Vector2.zero;
        Color color = m_rangeRenderere.color;
        color.a = 0.0f;
        m_rangeRenderere.color = color;
        m_rangeAttack.transform.eulerAngles = Vector3.zero;
        m_rangeAttack.transform.localPosition = Vector3.zero;
        m_rangeAttack.gameObject.layer = 12;
        color = m_rangeRenderereB.material.color;
        color.a = 0.0f;
        m_rangeRenderereB.material.color = color;
        m_rangeAttackB.transform.eulerAngles = Vector3.zero;
        m_rangeAttackB.transform.localPosition = Vector3.zero;
        m_rangeAttackB.gameObject.layer = 12;
    }

    void DeadAllEnemy()
    {
        foreach (var enemyObj in m_enemies)
        {
            if (enemyObj.activeSelf == false) continue;
            var enemy = enemyObj.GetComponent<Enemy>();
            enemy.Damage(enemy.m_MaxHP);
        }
    }

    public override void Damage(int damage)
    {
        m_HP -= damage;
        if (m_HP > 0)
        {
            // 被弾時処理
        }
        else
        {
            // 召喚した敵をすべて破壊
            DeadAllEnemy();
            m_minionController.AddExperiencePoint(m_levelPoint);
            gameObject.SetActive(false);
            m_hpui.SetActive(false);
            // ターゲットを変更
            m_mantisMode = BOSS_DROGONFLY_MODE.DEAD;
            m_mode = ENEMY_MODE.DEAD;
            m_minionController.ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);
            m_enemyController.StartCoroutine(m_enemyController.ChangeScene());
        }
    }
}
