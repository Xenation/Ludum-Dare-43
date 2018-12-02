using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public enum MenuChoice
    {
        Play,
        Credit,
        Quit
    }

    public enum LastInput
    {
        Nothing,
        Down,
        Up
    }

    [Serializable]
    public struct MenuChoiceInfos
    {
        public MenuChoice Type;
        public SpriteRenderer Renderer;
        public Sprite DefaultSprite;
        public Sprite HoverSprite;
        public Sprite SelectedSprite;
    }

    public class MenuController : MonoBehaviour
    {
        [SerializeField] private List<MenuChoiceInfos> m_infos;
        [SerializeField] private AudioSource m_changeButtonSound;
        [SerializeField] private AudioSource m_validateButtonSound;

        private int m_currentChoiceIndex = 0;

        private LastInput m_lastInput = LastInput.Nothing;
        private bool m_isExecuting = false;

        private void Start()
        {
            m_isExecuting = false;
        }

        private void Update()
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                if (m_lastInput != LastInput.Up)
                {
                    if (m_changeButtonSound)
                        m_changeButtonSound.Play();

                    m_currentChoiceIndex++;

                    if (m_currentChoiceIndex >= m_infos.Count)
                        m_currentChoiceIndex = 0;

                    m_lastInput = LastInput.Up;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                if (m_lastInput != LastInput.Down)
                {
                    if (m_changeButtonSound)
                        m_changeButtonSound.Play();

                    m_currentChoiceIndex--;

                    if (m_currentChoiceIndex < 0)
                        m_currentChoiceIndex = m_infos.Count - 1;

                    m_lastInput = LastInput.Down;
                }
            }
            else
            {
                m_lastInput = LastInput.Nothing;
            }

            if (Input.GetButtonDown("Submit"))
            {
                if (m_validateButtonSound)
                    m_validateButtonSound.Play();

                m_isExecuting = true;
                Execute();
            }

            if (!m_isExecuting)
            {
                for (int i = 0; i < m_infos.Count; i++)
                {
                    m_infos[i].Renderer.sprite = i == m_currentChoiceIndex ? m_infos[i].HoverSprite : m_infos[i].DefaultSprite;
                }

            }
        }

        private void Execute()
        {
            m_infos[m_currentChoiceIndex].Renderer.sprite = m_infos[m_currentChoiceIndex].SelectedSprite;
            switch (m_infos[m_currentChoiceIndex].Type)
            {
                case MenuChoice.Play:
                    GameManager.NextLevel();
                    break;
                case MenuChoice.Credit:
                    GameManager.NextLevel(100); // credit will probably be the last scene
                    break;
                case MenuChoice.Quit:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }

        IEnumerator ExecuteAfterSeconds(float seconds, int sceneToLoad = -1)
        {
            m_infos[m_currentChoiceIndex].Renderer.sprite = m_infos[m_currentChoiceIndex].SelectedSprite;
            yield return new WaitForSeconds(seconds);

            if (sceneToLoad == -2)
                Application.Quit();
            else if (sceneToLoad == -1)
                GameManager.NextLevel();
            else
                GameManager.NextLevel(sceneToLoad);

        }

    }
}
