using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionHPUI : MonoBehaviour
{
    public Minion m_minion = null;
    [SerializeField]
    private Slider m_slider = null;
    Vector3 m_angle = Vector3.zero;
    RectTransform m_rect = null;
    [SerializeField]
    private float m_offsetY = 1.0f;

    private void Start()
    {
        m_slider.maxValue = m_minion.m_HP;
        m_rect = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        m_slider.value = m_minion.m_HP;
        m_rect.transform.position = Camera.main.WorldToScreenPoint(
            new Vector3(m_minion.transform.position.x, m_minion.transform.position.y + m_offsetY, 0.0f));
    }
}
