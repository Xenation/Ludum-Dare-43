using UnityEngine;

namespace LD43
{
    public class PressButton : MonoBehaviour
    {
        [SerializeField] private Activable m_activable;
        [SerializeField] private AudioSource m_pressSound;
        [SerializeField] private AudioSource m_unpressSound;

        private int m_pressedTime = 0;
        private bool m_isPressed = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable")))
            {
                if (!m_isPressed)
                {
					BoxCollider2D col = GetComponent<BoxCollider2D>();
					Vector3 colCenter = transform.TransformPoint(col.offset);
					transform.position = transform.position + Vector3.down * 0.1f;
					col.offset = transform.InverseTransformPoint(col.bounds.center);
					if (m_activable && m_pressedTime == 0)
                    {
                    if (m_unpressSound && m_unpressSound.isPlaying)
                        m_unpressSound.Stop();

                    if (m_pressSound && !m_pressSound.isPlaying)
                        m_pressSound.Play();

                        m_activable.Activate();
                        m_isPressed = true;
                    } 
                }

                m_pressedTime++;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if ((collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable")))
            {
				BoxCollider2D col = GetComponent<BoxCollider2D>();
				Vector3 colCenter = transform.TransformPoint(col.offset);
				transform.position = transform.position + Vector3.up * 0.1f;
				col.offset = transform.InverseTransformPoint(col.bounds.center);
				if (m_isPressed)
                {
                    if (m_pressedTime <= 1 && m_activable)
                    {
                    if (m_pressSound && m_pressSound.isPlaying)
                        m_pressSound.Stop();

                    if (m_unpressSound && !m_unpressSound.isPlaying)
                        m_unpressSound.Play();

                        m_activable.Desactivate();
                        m_isPressed = false;
                    } 
                }
                m_pressedTime--;
            }
        }

    }
}
