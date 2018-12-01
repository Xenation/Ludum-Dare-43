using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pike : MonoBehaviour
{
    [SerializeField] private List<string> m_activableTags = new List<string> { "Players" };

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_activableTags.Contains(collision.collider.tag))
        {
            // TODO : kill the character
        }
    }
    
}
