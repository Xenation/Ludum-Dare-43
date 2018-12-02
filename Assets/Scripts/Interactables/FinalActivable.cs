using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD43
{

    public class FinalActivable : Activable
    {
        [Header("Final")]
        [SerializeField] private int m_activationNeededBeforeOpen = 5;
        [SerializeField] private float m_timeAnim = 3.0f;
        [SerializeField] private float m_timeFade = 5.0f;
        [SerializeField] private RawImage m_fade;

        private int m_currentActivationNumber = 0;
        private Animator m_anim;

        private void Start()
        {
            m_anim = GetComponent<Animator>();
        }

        public override void Activate()
        {
            m_currentActivationNumber++;
            if (m_currentActivationNumber >= m_activationNeededBeforeOpen)
                StartCoroutine(Finish());
        }

        public override void Desactivate()
        {
            m_currentActivationNumber--;
        }

        private IEnumerator Finish()
        {
            // TODO : anim porte 
            if (m_activateSound)
                m_activateSound.Play();

            GameManager.PlayEndingMusic();

            yield return new WaitForSeconds(m_timeAnim);
            if (m_activateSound)
                m_activateSound.Stop();

            float current = 0.0f;
            float start = 0f;
            float end = 1f;

            while (current < m_timeFade)
            {
                current += (Time.deltaTime );
                if (m_fade)
                {
                    float alpha = Mathf.Lerp(start, end, current / m_timeFade);
                    m_fade.color = new Color(m_fade.color.r, m_fade.color.g, m_fade.color.b, alpha);
                }

                yield return null;
            }

            GameManager.NextLevel();
        }
    }

}