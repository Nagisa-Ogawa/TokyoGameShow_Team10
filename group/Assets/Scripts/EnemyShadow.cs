using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShadow : MonoBehaviour
{
    [SerializeField]
    private GameObject m_enemyObj = null;
    [SerializeField]
    private Enemy m_enemy = null;
    [SerializeField]
    private SpriteRenderer m_sRendere = null;
    [SerializeField]
    private float m_goTime = 1.0f; // 敵に行くまでの時間
    [SerializeField]
    private float m_returnTime = 1.0f;  // 帰るまでの時間
    [SerializeField]
    private float m_waitStartTime = 1.0f; // 攻撃を開始するまでの待機時間
    [SerializeField]
    private float m_waitEndTime = 1.0f; // 攻撃を終えてから次に行動するまでの待機時間
    private float m_nowTime = 0.0f;
    private Vector3 m_targetPos = Vector3.zero;
    private Vector3 m_startPos = Vector3.zero;
    private Coroutine m_Coroutine = null;
    private GameObject m_target = null;

    public void StartAttack(GameObject target)
    {
        m_target = target;
        if (m_enemy.GetComponent<SpriteRenderer>().flipX == true) m_sRendere.flipX = true;
        if (m_enemy.GetComponent<SpriteRenderer>().flipX == false) m_sRendere.flipX = false;
        // 待機
        if (gameObject.activeSelf)
        {
            m_Coroutine = StartCoroutine(WaitStart());
        }
    }

    public void StartMove_Go()
    {
        m_targetPos = m_target.transform.position;
        m_startPos=m_enemyObj.transform.position;
        m_nowTime = 0.0f;
        if (gameObject.activeSelf)
        {
            m_Coroutine = StartCoroutine(GoMove());
        }
    }

    public void StartMove_Return()
    {
        m_startPos = transform.position;
        m_targetPos=m_enemyObj.transform.position;
        m_nowTime = 0.0f;
        if (gameObject.activeSelf)
        {
            m_Coroutine = StartCoroutine(ReturnMove());
        }
    }

    public void Hit()
    {
        // ダメージ処理
        m_enemy.DamageTarget();
        StartMove_Return();
    }

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(m_waitStartTime);
        StartMove_Go();
    }

    // 敵に行く残像のコルーチン
    public IEnumerator GoMove()
    {
        while (true)
        {
            m_nowTime += Time.deltaTime;
            if(m_nowTime > m_goTime)
            {
                transform.position = m_targetPos;
                Hit();
                yield break;
            }
            float timeRate=m_nowTime/m_goTime;
            transform.position = Vector3.Lerp(m_startPos, m_targetPos, timeRate);
            yield return null;
        }
    }

    public IEnumerator ReturnMove()
    {
        while (true)
        {
            m_nowTime += Time.deltaTime;
            if (m_nowTime > m_returnTime)
            {
                transform.position = m_targetPos;
                // 移動が終わったことを通知
                if (gameObject.activeSelf)
                {
                    m_Coroutine = StartCoroutine(EndWait());
                }
                yield break;
            }
            float timeRate = m_nowTime / m_returnTime;
            transform.position = Vector3.Lerp(m_startPos, m_targetPos, timeRate);
            yield return null;
        }
    }

    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(m_waitEndTime);
        m_enemy.EndAttack();
    }

    public void ResetAttack()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
        }
        m_nowTime = 0;
        transform.position = m_enemyObj.transform.position;
    }
}
