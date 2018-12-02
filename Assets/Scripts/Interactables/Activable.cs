using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public class Activable : MonoBehaviour
    {
        public enum ActivableState
        {
            Openned,
            Openning,
            Closed,
            Closing
        }

        [SerializeField] private Transform m_model;
        [SerializeField] private Transform m_startPosition;
        [SerializeField] private Transform m_endPosition;

        [Header("Anim time")]
        [SerializeField] private float m_animOpenSeconds = 1.0f;
        [SerializeField] private float m_animCloseSeconds = 3.0f;
        [SerializeField] private float m_animOpenDelaySeconds = 0.0f;
        [SerializeField] private float m_animCloseDelaySeconds = 0.5f;

        [Header("Sound")]
        [SerializeField] private AudioSource m_activateSound;
        [SerializeField] private AudioSource m_desactivateSound;

        private bool m_isMoving = false;
        private float m_currentLerpRatio = 0.0f;
        private ActivableState m_state = ActivableState.Closed;

        public void Activate()
        {
            if (m_state > ActivableState.Openning)
            {
                StopAllCoroutines();

                if (m_state == ActivableState.Closing && m_desactivateSound)
                    SoundHelper.I.StopWithFade(m_desactivateSound);

                StartCoroutine(AnimateActivable(m_animOpenSeconds, true));
            }

        }

        public void Desactivate()
        {
            if (m_state < ActivableState.Closed)
            {
                StopAllCoroutines();

                if (m_state == ActivableState.Openning && m_activateSound)
                    SoundHelper.I.StopWithFade(m_activateSound);

                StartCoroutine(AnimateActivable(m_animCloseSeconds, false));
            }

        }

        IEnumerator AnimateActivable(float timeAnime, bool open)
        {
            if (open)
            {
                m_state = ActivableState.Openning;
                if (m_activateSound)
                    m_activateSound.Play();
                yield return new WaitForSeconds(m_animOpenDelaySeconds);
            }
            else
            {
                m_state = ActivableState.Closing;
                if (m_desactivateSound)
                    m_desactivateSound.Play();
                yield return new WaitForSeconds(m_animCloseDelaySeconds);
            }

            bool finishAnim = false;

            while (!finishAnim)
            {
                float dtLerp = (1f / timeAnime) * Time.deltaTime * (open ? 1 : -1);
                m_currentLerpRatio += dtLerp;
                m_model.position = Vector3.Lerp(m_startPosition.position, m_endPosition.position, m_currentLerpRatio);

                if ((open && m_currentLerpRatio >= 1) || (!open && m_currentLerpRatio <= 0))
                    finishAnim = true;

                yield return null;
            }

            if (open)
            {
                m_state = ActivableState.Openned;
                SoundHelper.I.StopWithFade(m_activateSound);
            }
            else
            {
                m_state = ActivableState.Closed;
                SoundHelper.I.StopWithFade(m_desactivateSound);
            }

            m_currentLerpRatio = Mathf.Clamp01(m_currentLerpRatio);
            yield return null;
        }
    }
}
