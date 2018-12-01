using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activable : MonoBehaviour {
    [SerializeField] private Transform m_model;
    [SerializeField] private Transform m_startPosition;
    [SerializeField] private Transform m_endPosition;
    [SerializeField] private float m_animTimeSeconds = 1.0f;

    private bool m_isMoving = false;
    private bool m_isOpen = false;

    private float m_currentLerpRatio = 0.0f;

    public void Activate()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateActivable(m_animTimeSeconds, true));
    }

    public void Desactivate()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateActivable(m_animTimeSeconds, false));
    }

    IEnumerator AnimateActivable(float timeAnime, bool open)
    {
        if (m_isMoving && open != m_isOpen) // we change during anim
            m_currentLerpRatio = 1 - m_currentLerpRatio;

        m_isMoving = true;
        bool finishAnim = false;

        Vector3 start = open ? m_startPosition.position : m_endPosition.position;
        Vector3 end = open ? m_endPosition.position : m_startPosition.position;

        while (!finishAnim)
        {
            m_currentLerpRatio += m_animTimeSeconds * Time.deltaTime;
            m_model.position = Vector3.Lerp(start, end, m_currentLerpRatio);

            if (m_currentLerpRatio >= 1)
                finishAnim = true;
            yield return null;
        }

        m_currentLerpRatio = 0.0f;
        m_isOpen = open;
        m_isMoving = false;
        yield return null;
    }
}
