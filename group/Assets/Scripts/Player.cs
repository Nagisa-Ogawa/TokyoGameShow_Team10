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
    [SerializeField]
    private MinionController m_minionController = null;
    private Vector2 m_dir = Vector2.zero;
    Rigidbody2D rigidBody;
    [SerializeField]
    private float m_returnDistance = 10.0f;
    public float m_ReturnDistance { get { return m_returnDistance; } private set { m_returnDistance = value; } }

    // �U���֘A
    [SerializeField]
    private PlayerAttack m_pAttack = null;  // �v���C���[�̍U���͈̓I�u�W�F�N�g
    [SerializeField]
    private int m_damage = 1;   
    public int m_Damage { get { return m_damage; }private set { m_damage = value; } }
    [SerializeField]
    private float m_attackCoolDown = 2.0f;    // ���ɍU���ł���܂ł̃N�[���_�E��
    [SerializeField]
    private float m_hitTime = 0.5f;   // �U���̓����蔻������鎞��
    [SerializeField]
    private float m_drawTime = 1.0f;
    private bool isCanAttack = true;    // �U���ł��邩�ǂ����̃t���O
    public Vector2 m_attackDir { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody=GetComponent<Rigidbody2D>();
        m_pAttack.gameObject.SetActive(false);
        m_attackDir = Vector2.zero;
    }

    private void OnMove(InputValue value)
    {
        // MoveAction�̓��͒l���擾
        m_dir = value.Get<Vector2>();
        if(m_dir != Vector2.zero)
        {
            m_attackDir = m_dir;
        }
    }

    private void OnFOLLOW()
    {
        m_minionController.ChangeMode(Minion.MINION_MODE.FOLLOW);
    }

    private void OnATTACK()
    {
        m_minionController.ChangeMode(Minion.MINION_MODE.MOVE_ENEMY);

    }

    private void OnESCAPE()
    {
        m_minionController.ChangeMode(Minion.MINION_MODE.ESCAPE);
    }

    private void OnPLAYER_ATTACK()
    {
        if(isCanAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isCanAttack = false;
        // �U�������փI�u�W�F�N�g���Z�b�g
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
    //void Update()
    //{
    //}

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
        SceneManager.LoadScene("GameOver");
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(m_dir.x, m_dir.y) * m_speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Home")
        {
            // ����ł���~�j�I���𕜊�
            m_minionController.RevivalMinion(collision.transform.position);
        }
    }
}
