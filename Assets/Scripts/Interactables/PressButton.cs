using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour {

    private GameObject m_door;
    private Transform m_startPosition;
    private Transform m_endPosition;

    private bool isMoving = false;
    private bool isOpen = false; 

    private float m_currentLerpRatio = 0.0f;
    [SerializeField] private float m_animTimeSeconds = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        AnimateActivable(m_animTimeSeconds, true);
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        AnimateActivable(m_animTimeSeconds, false);
    }

    IEnumerator AnimateActivable(float timeAnime, bool open)
    {
        if (isMoving) // we change during anim
            m_currentLerpRatio = 1 - m_currentLerpRatio;

        isMoving = true;
        bool finishAnim = false;

        Vector3 start = open ? m_startPosition.position : m_endPosition.position;
        Vector3 end = open ? m_endPosition.position : m_startPosition.position;

        while (finishAnim)
        {
            m_currentLerpRatio += m_animTimeSeconds * Time.deltaTime;
            m_door.transform.position = Vector3.Lerp(start, end, m_currentLerpRatio);

            if (m_currentLerpRatio >= 1)
                finishAnim = true;
            yield return null;
        }

        m_currentLerpRatio = 1;
        isMoving = false;
        yield return null;
    }

}
