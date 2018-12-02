using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public class SingingObject : MonoBehaviour
    {
        [SerializeField] private AudioSource m_sound;

        private bool m_lastFrameSing = false;
        private Rigidbody2D m_body;
        private void Start()
        {
            m_body = GetComponent<Rigidbody2D>();
        }
        private void Update()
        {
            if (m_body)
            {
                if (m_body.velocity.sqrMagnitude > 0)
                {
                    if (!m_lastFrameSing && m_sound)
                        m_sound.Play();

                    m_lastFrameSing = true;
                }
                else
                {
                    if (m_lastFrameSing && m_sound)
                        SoundHelper.I.StopWithFade(m_sound);

                    m_lastFrameSing = false;
                }
            }
        }
    }
}
