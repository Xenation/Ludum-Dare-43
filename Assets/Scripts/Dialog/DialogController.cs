using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD43
{
    [Serializable]
    public struct DialogInfos
    {
        [TextArea] public string Content;
        [TextArea] public string ContentWithEveryoneAlive;
        public PlayerTypesFlag Character;
    }

    [Serializable]
    public struct PlayersNames
    {
        public string Name;
        public PlayerTypesFlag Character;
    }

    public class DialogController : MonoBehaviour
    {
        [SerializeField] private List<DialogInfos> m_texts;
        [SerializeField] private List<PlayersNames> m_names;
        [SerializeField] private GameObject m_bubblePrefab;
        [SerializeField] private Vector3 m_bubbleOffset = new Vector3(0f, 1f, -0.5f);
        [SerializeField] private float m_timeBetweenLetters = 0.1f;
        [SerializeField] private AudioSource m_soundChangeDialog;
        [SerializeField] private AudioSource m_soundDialog;

        private GameObject m_currentBubble;
        private TextMeshPro m_bubbleText;
        private TextMeshPro m_bubbleName;
        private PlayerTypesFlag m_charactersDisplaying = 0;
        private Transform m_toFollow = null;

        public PlayerTypesFlag CharactersDisplaying { get { return m_charactersDisplaying; } }


        private int m_currentIndex = -1;

        private bool m_isDisplayingText = false;
        private bool m_wantToStopDisplaying = false;

        private void Update()
        {
            if (m_toFollow)
                m_currentBubble.transform.position = m_toFollow.position + m_bubbleOffset;

            if (m_charactersDisplaying > 0 && Input.anyKeyDown)
            {
                if (m_isDisplayingText)
                    m_wantToStopDisplaying = true;
                else
                    NextText();
            }
        }

        private void Start()
        {
            m_currentBubble = Instantiate(m_bubblePrefab);
            DontDestroyOnLoad(m_currentBubble);
            m_currentBubble.SetActive(false);

            TextMeshPro[] texts = m_currentBubble.GetComponentsInChildren<TextMeshPro>();
            foreach (var text in texts)
            {
                if (text.name.Contains("text"))
                    m_bubbleText = text;
                else if (text.name.Contains("name"))
                    m_bubbleName = text;
            }
            m_bubbleText.renderer.sortingOrder = 100;
            m_bubbleName.renderer.sortingOrder = 100;
        }

        public void NextText()
        {
            if (CharactersManager.I)
            {
                m_currentIndex++;
                if (m_currentIndex >= m_texts.Count || (string.IsNullOrEmpty(m_texts[m_currentIndex].Content) && string.IsNullOrEmpty(m_texts[m_currentIndex].ContentWithEveryoneAlive)))
                    ChangeUIVisibility(0);
                else
                    ChangeUIVisibility(m_texts[m_currentIndex].Character, GameManager.PlayerHereAtStart == (PlayerTypesFlag)31 ? m_texts[m_currentIndex].ContentWithEveryoneAlive : m_texts[m_currentIndex].Content, m_names.FirstOrDefault(n => n.Character == m_texts[m_currentIndex].Character).Name ?? "");
            }

        }

        private void ChangeUIVisibility(PlayerTypesFlag charactersDisplaying, string text = "", string name = "")
        {
            if (charactersDisplaying == 0)
            {
                m_toFollow = null;
                m_currentBubble.SetActive(false);
                m_charactersDisplaying = charactersDisplaying;
                return;
            }


            CharController controller = CharactersManager.I.GetCharacterWithType(charactersDisplaying);
            if (controller && !string.IsNullOrEmpty(text))
            {
                if (m_soundChangeDialog)
                    m_soundChangeDialog.Play();

                m_toFollow = controller.OverlayPosition.transform;
                m_bubbleName.text = name;

                StopAllCoroutines();
                StartCoroutine(DisplayText(text, m_timeBetweenLetters));

                m_charactersDisplaying = charactersDisplaying;

                if (m_toFollow)
                    m_currentBubble.transform.position = m_toFollow.position + m_bubbleOffset;

                if (!m_currentBubble.activeSelf)
                    m_currentBubble.SetActive(true);
            }
            else
            {
                NextText();
            }


        }

        IEnumerator DisplayText(string text, float timeBetweenLetter)
        {
            m_isDisplayingText = true;

            if (m_soundDialog)
                m_soundDialog.Play();

            string currentDisplay = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (m_wantToStopDisplaying)
                {
                    m_bubbleText.text = text;
                    m_isDisplayingText = false;
                    m_wantToStopDisplaying = false;

                    if (m_soundDialog)
                        SoundHelper.I.StopWithFade(m_soundDialog);
                    yield break;
                }

                currentDisplay += text[i];
                m_bubbleText.text = currentDisplay;
                yield return new WaitForSeconds(timeBetweenLetter);
            }

            m_bubbleText.text = text;
            m_isDisplayingText = false;
            m_wantToStopDisplaying = false;

            if (m_soundDialog)
                SoundHelper.I.StopWithFade(m_soundDialog);
            yield return null;
        }

    }
}
