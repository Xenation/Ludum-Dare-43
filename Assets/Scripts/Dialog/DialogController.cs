using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {
    [SerializeField, TextArea] private List<string> m_texts;

    private TextMeshProUGUI m_dialogText;
    private RawImage m_background;
    private int m_currentIndex = -1;

    private bool m_displayingUI = false;

    private void Start()
    {
        m_dialogText = GetComponentInChildren<TextMeshProUGUI>();
        m_background = GetComponentInChildren<RawImage>();

        ChangeUIVisibility(false);
    }

    public void NextText()
    {
        m_currentIndex++;
        if (m_currentIndex >= m_texts.Count || string.IsNullOrEmpty(m_texts[m_currentIndex]))
        {
            ChangeUIVisibility(false);
        }
        else
        {
            if (!m_displayingUI)
                ChangeUIVisibility(true);

            m_dialogText.text = m_texts[m_currentIndex];
        }

    }

    private void ChangeUIVisibility(bool isVisible)
    {
        m_displayingUI = isVisible;

        m_dialogText.enabled = m_displayingUI;
        m_background.enabled = m_displayingUI;
    }

}
