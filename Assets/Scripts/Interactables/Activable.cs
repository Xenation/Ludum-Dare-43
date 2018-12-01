using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activable : MonoBehaviour
{
    [SerializeField] private Transform m_model;
    [SerializeField] private Transform m_startPosition;
    [SerializeField] private Transform m_endPosition;
    [SerializeField] private float m_animTimeSeconds = 1.0f;

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
        bool finishAnim = false;

        while (!finishAnim)
        {
            float dtLerp = m_animTimeSeconds * Time.deltaTime * (open ? 1 : -1);
            m_currentLerpRatio += dtLerp;
            m_model.position = Vector3.Lerp(m_startPosition.position, m_endPosition.position, m_currentLerpRatio);

            if ((open && m_currentLerpRatio >= 1) || (!open && m_currentLerpRatio <= 0))
                finishAnim = true;

            yield return null;
        }

        m_currentLerpRatio = Mathf.Clamp01(m_currentLerpRatio);
        yield return null;
    }
}
