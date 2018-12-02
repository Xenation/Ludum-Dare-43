using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public enum MenuChoice
    {
        Play,
        Quit
    }

    public enum LastInput
    {
        Nothing,
        Down,
        Up
    }

    public class MenuController : MonoBehaviour
    {
        private LastInput m_lastInput = LastInput.Nothing;
        private MenuChoice m_choice = MenuChoice.Play;

        [SerializeField] private GameObject[] m_titles;
        [SerializeField] private GameObject m_indicator;
        [SerializeField] private Vector3 m_offsetIndicator = new Vector3(-5.0f, -1.0f, 0f);

        private void Start()
        {
            UpdateIndicator();
        }

        private void Update()
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                if (m_lastInput != LastInput.Up)
                {
                    if (m_choice == MenuChoice.Quit)
                        m_choice = MenuChoice.Play;
                    else
                        m_choice++;

                    UpdateIndicator();
                    m_lastInput = LastInput.Up;
                }
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                if (m_lastInput != LastInput.Down)
                {
                    if (m_choice == MenuChoice.Play)
                        m_choice = MenuChoice.Quit;
                    else
                        m_choice--;

                    UpdateIndicator();
                    m_lastInput = LastInput.Down;
                }
            }
            else
            {
                m_lastInput = LastInput.Nothing;
            }

            if (Input.GetButtonDown("Submit"))
                Execute();
        }

        private void UpdateIndicator()
        {
            if (m_titles != null)
            {
                for (int i = 0; i < m_titles.Length; i++)
                {
                    if (i == (int)m_choice)
                        m_indicator.transform.position = new Vector3(m_offsetIndicator.x, m_titles[i].transform.position.y + m_offsetIndicator.y, 0f);
                }
            }
        }

        private void Execute()
        {
            switch (m_choice)
            {
                case MenuChoice.Play:
                    GameManager.NextLevel();
                    break;
                case MenuChoice.Quit:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
    } 
}
