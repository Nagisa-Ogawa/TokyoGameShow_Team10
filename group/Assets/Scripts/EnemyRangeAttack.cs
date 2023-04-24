using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
    [SerializeField]
    private Enemy m_enemy = null;
    private List<Minion> m_hitMinionList = new List<Minion>();
    public List<Minion> m_HitMinionList { get { return m_hitMinionList; } private set { m_hitMinionList = value; } }
    [SerializeField]
    private float m_rangeDistance = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetRange(Minion minion)
    {
        transform.position += (minion.transform.position - transform.position).normalized * m_rangeDistance;
        var rot = Quaternion.FromToRotation(Vector3.up,(minion.transform.position-transform.position) );
        transform.rotation = rot;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Minion" && 
                m_hitMinionList.Contains(collision.GetComponent<Minion>()) == false)
        {
            m_hitMinionList.Add(collision.GetComponent<Minion>());
            Debug.Log("入った　：　" + m_HitMinionList.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Minion" &&
                m_hitMinionList.Contains(collision.GetComponent<Minion>()) == true)
        {
            m_hitMinionList.Remove(collision.GetComponent<Minion>());
            Debug.Log("でた　：　" + m_HitMinionList.Count);
        }
    }
}
