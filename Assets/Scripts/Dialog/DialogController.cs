using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD43
{
    [Serializable]
    public struct DialogInfos
    {
        [TextArea] public string Content;
        public PlayerTypesFlag Character;
    }

    public class DialogController : MonoBehaviour
    {
        [SerializeField] private List<DialogInfos> m_texts;
        [SerializeField] private GameObject m_bubblePrefab;
        [SerializeField] private Vector3 m_bubbleOffset = new Vector3(0f, 1f, -0.5f);
        [SerializeField] private float m_timeBetweenLetters = 0.1f;

        private GameObject m_currentBubble;
        private TextMeshPro m_bubbleText;
        private PlayerTypesFlag m_charactersDisplaying = 0;

        private int m_currentIndex = -1;

        private bool m_isDisplayingText = false;
        private bool m_wantToStopDisplaying = false;

        private void Update()
        {
            if (m_charactersDisplaying > 0 && Input.anyKeyDown)
            {
                if (m_isDisplayingText)
                {
                    m_wantToStopDisplaying = true;
                }
                else
                {
                    NextText();
                }
            }
        }

        private void Start()
        {
            m_currentBubble = Instantiate(m_bubblePrefab);
            m_bubbleText = m_currentBubble.GetComponentInChildren<TextMeshPro>();
            m_bubbleText.renderer.sortingOrder = 100;
            ChangeUIVisibility(0);
        }

        public void NextText()
        {
            m_currentIndex++;
            if (m_currentIndex >= m_texts.Count || string.IsNullOrEmpty(m_texts[m_currentIndex].Content))
                ChangeUIVisibility(0);
            else
                ChangeUIVisibility(m_texts[m_currentIndex].Character, m_texts[m_currentIndex].Content);

        }

        private void ChangeUIVisibility(PlayerTypesFlag charactersDisplaying, string text = "")
        {
            if (charactersDisplaying == 0)
            {
                m_currentBubble.SetActive(false);
                return;
            }

            CharController controller = CharactersManager.I.GetCharacterWithType(charactersDisplaying);
            if (controller)
            {
                if (!m_currentBubble.activeSelf)
                    m_currentBubble.SetActive(true);

                m_currentBubble.transform.parent = controller.OverlayPosition.transform;
                m_currentBubble.transform.localPosition = m_bubbleOffset;

                StopAllCoroutines();
                StartCoroutine(DisplayText(text, m_timeBetweenLetters));

                m_charactersDisplaying = charactersDisplaying;
            }
            else
            {
                NextText();
            }

        }

        IEnumerator DisplayText(string text, float timeBetweenLetter)
        {
            m_isDisplayingText = true;
            string currentDisplay = "";
            for (int i = 0; i < text.Length; i++)
            {
                if(m_wantToStopDisplaying)
                {
                    m_bubbleText.text = text;
                    m_isDisplayingText = false;
                    m_wantToStopDisplaying = false;
                     yield break;
                }

                currentDisplay += text[i];
                m_bubbleText.text = currentDisplay;
                yield return new WaitForSeconds(timeBetweenLetter);
            }

            m_bubbleText.text = text;
            m_isDisplayingText = false;
            m_wantToStopDisplaying = false;
            yield return null;
        }

    }
}
