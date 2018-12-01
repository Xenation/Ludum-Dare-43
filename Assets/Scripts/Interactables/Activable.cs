using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activable : MonoBehaviour {
    [SerializeField] private Transform m_model;
    [SerializeField] private Transform m_startPosition;
    [SerializeField] private Transform m_endPosition;
    [SerializeField] private float m_animTimeSeconds = 1.0f;

    private bool isMoving = false;
    private bool isOpen = false;

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
        if (isMoving) // we change during anim
            m_currentLerpRatio = 1 - m_currentLerpRatio;

        isMoving = true;
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
        isOpen = open;
        isMoving = false;
        yield return null;
    }
}
