using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 2.0f;
    [SerializeField]
    private int m_hp = 5;
    public int m_HP { get { return m_hp; } private set { m_hp = value; } }
    private int m_maxHP = 0;
    [SerializeField]
    private MinionController m_minionController = null;
    [SerializeField]
    private EnemyController m_enemyController = null;
    private Vector2 m_dir = Vector2.zero;
    Rigidbody2D rigidBody;
    [SerializeField]
    private float m_returnDistance = 10.0f;
    public float m_ReturnDistance { get { return m_returnDistance; } private set { m_returnDistance = value; } }
    [SerializeField]
    private SpriteRenderer m_sRenderer = null;

    // 攻撃関連
    [SerializeField]
    private PlayerAttack m_pAttack = null;  // プレイヤーの攻撃範囲オブジェクト
    [SerializeField]
    private int m_damage = 1;   
    public int m_Damage { get { return m_damage; }private set { m_damage = value; } }
    [SerializeField]
    private float m_attackCoolDown = 2.0f;    // 次に攻撃できるまでのクールダウン
    [SerializeField]
    private float m_hitTime = 0.5f;   // 攻撃の当たり判定をする時間
    [SerializeField]
    private float m_drawTime = 1.0f;
    private bool isCanAttack = true;    // 攻撃できるかどうかのフラグ
    public Vector2 m_attackDir { get; private set; }

    // レベルアップ関係
    private bool m_isLevelUp = false;
    [SerializeField]
    private LevelUpUI m_levelUpUI = null;
    private bool m_canLevelUp=false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = GameObject.FindGameObjectWithTag("Home").transform.position;
        rigidBody=GetComponent<Rigidbody2D>();
        m_pAttack.gameObject.SetActive(false);
        m_attackDir = Vector2.zero;
        m_maxHP = m_hp;
    }

    private void OnMove(InputValue value)
    {
        if (m_isLevelUp)
        {
        }
        else
        {
            m_dir = value.Get<Vector2>();
            // MoveActionの入力値を取得
            if (m_dir != Vector2.zero)
            {
                m_attackDir = m_dir;
            }
            else
            {
                m_attackDir = Vector2.up;
            }
        }
    }

    private void OnFOLLOW()
    {
        if (m_isLevelUp)
        {
            m_levelUpUI.Decide();
        }
        else
                {
            m_minionController.ChangeMode(Minion.MINION_MODE.FOLLOW);
        }
    }

    private void OnATTACK()
    {
        if (m_isLevelUp)
        {
            m_isLevelUp = false;
            m_levelUpUI.Uninit();
        }
        else
        {
            m_minionController.ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);
        }
    }

    private void OnESCAPE()
    {
        if(m_isLevelUp)
        {

        }
        else
        {
            m_minionController.ChangeMode(Minion.MINION_MODE.ESCAPE);
        }
    }

    private void OnUI_OPEN()
    {
        if (m_canLevelUp&&!m_isLevelUp)
        {
            m_isLevelUp = true;
            m_levelUpUI.Init();
            m_dir = Vector2.zero;
        }
        else if (m_isLevelUp)
        {
            m_isLevelUp = false;
            m_levelUpUI.Uninit();
        }

    }

    private void OnUI_UP()
    {
        if (m_isLevelUp)
        {
            m_levelUpUI.Move_Up();
        }
    }

    private void OnUI_DOWN()
    {
        if (m_isLevelUp)
        {
            m_levelUpUI.Move_Down();
        }
    }

    private void OnPLAYER_ATTACK()
    {
        //if(isCanAttack)
        //{
        //    StartCoroutine(Attack());
        //}
    }

    IEnumerator Attack()
    {
        isCanAttack = false;
        // 攻撃方向へオブジェクトをセット
        m_pAttack.Set(m_dir);
        m_pAttack.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_hitTime);
        m_pAttack.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(m_drawTime-m_hitTime);
        m_pAttack.GetComponent <BoxCollider2D>().enabled = true;
        m_pAttack.gameObject.SetActive(false);
        m_pAttack.transform.localPosition= Vector3.zero;
        m_pAttack.transform.eulerAngles = Vector3.zero;
        StartCoroutine(CheckCanAttack());
        yield break;
    }

    IEnumerator CheckCanAttack()
    {
        yield return new WaitForSeconds(m_attackCoolDown);
        isCanAttack=true;
    }
    //// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_minionController.ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);
        }
    }

    public void Damage(int damage)
    {
        m_hp -= damage;
        if(m_hp>0)
        {

        }
        else
        {
            m_hp = 0;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("GameOver 1");
    }

    private void FixedUpdate()
    {
        if (m_dir.x > 0.0f && m_sRenderer.flipX == false) m_sRenderer.flipX = true;
        if (m_dir.x < 0.0f && m_sRenderer.flipX == true) m_sRenderer.flipX = false;
        rigidBody.velocity = new Vector2(m_dir.x, m_dir.y) * m_speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Home")
        {
            m_hp = m_maxHP;
            // 死んでいるミニオンを復活
            m_minionController.RevivalMinion();
            // 敵も復活
            m_enemyController.ActiveAll();
            m_canLevelUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Home")
        {
            m_canLevelUp = false;
        }
    }
}
