using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public class CreditController : MonoBehaviour
    {

        [SerializeField] private SpriteRenderer m_renderer;
        [SerializeField] private Sprite m_selected;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Submit"))
            {
                m_renderer.sprite = m_selected;
                GameManager.NextLevel();
            }
        }
    } 
}
