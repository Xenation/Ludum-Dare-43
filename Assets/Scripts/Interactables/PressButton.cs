using UnityEngine;

namespace LD43
{
    public class PressButton : MonoBehaviour
    {
        [SerializeField] private Activable m_activable;
        [SerializeField] private AudioSource m_pressSound;
        [SerializeField] private AudioSource m_unpressSound;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable"))
            {
                if (m_unpressSound && m_unpressSound.isPlaying)
                    m_unpressSound.Stop();

                if (m_pressSound && !m_pressSound.isPlaying)
                    m_pressSound.Play();

                m_activable.Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable"))
            {
                if (m_pressSound && m_pressSound.isPlaying)
                    m_pressSound.Stop();

                if (m_unpressSound && !m_unpressSound.isPlaying)
                    m_unpressSound.Play();
                m_activable.Desactivate();
            }
        }

    } 
}
