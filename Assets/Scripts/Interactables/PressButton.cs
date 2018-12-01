using UnityEngine;

namespace LD43
{
    public class PressButton : MonoBehaviour
    {
        [SerializeField] private Activable m_activable;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable"))
                m_activable.Activate();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<CharController>() || collision.gameObject.layer == LayerMask.NameToLayer("Movable"))
                m_activable.Desactivate();
        }

    } 
}
