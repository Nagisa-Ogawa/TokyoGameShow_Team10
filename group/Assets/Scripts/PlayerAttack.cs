using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Player m_player = null;
    [SerializeField]
    private float m_distance = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(Vector2 dir)
    {
        var rot = Quaternion.FromToRotation(Vector3.up, dir);
        transform.rotation = rot;
        transform.localPosition += transform.up * m_distance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().Damage(m_player.m_Damage);
        }
    }
}
