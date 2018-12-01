using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{

    [SerializeField] private Activable m_activable;
    [SerializeField] private List<string> m_activableTags = new List<string> { "Players" };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_activableTags.Contains(collision.tag))
            m_activable.Activate();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_activableTags.Contains(collision.tag))
            m_activable.Desactivate();
    }

}
