using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    public Player m_player = null;
    [SerializeField]
    private Slider m_slider = null;
    Vector3 m_angle = Vector3.zero;
    RectTransform m_rect = null;
    [SerializeField]
    private float m_offsetY = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_slider.maxValue = m_player.m_HP;
        m_rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        m_slider.value = m_player.m_HP;
        m_rect.transform.position = Camera.main.WorldToScreenPoint(
            new Vector3(m_player.transform.position.x, m_player.transform.position.y + m_offsetY, 0.0f));
    }
}
