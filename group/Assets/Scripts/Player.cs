using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float m_ReturnDistance
    { get { return m_returnDistance; }private set { m_returnDistance = value; } }
    // Start is called before the first frame update
    void Start()
    {
        rigidBody=GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value)
    {
        // MoveActionの入力値を取得
        m_dir = value.Get<Vector2>();

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
    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(m_dir.x, m_dir.y) * m_speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Home")
        {
            // 死んでいるミニオンを復活
            m_minionController.RevivalMinion(collision.transform.position);
        }
    }
}
