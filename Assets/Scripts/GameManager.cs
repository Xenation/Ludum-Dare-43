using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD43
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager m_instance;

        void Awake()
        {
            if (m_instance == null)
                m_instance = this;
            else if (m_instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            m_instance.Init();
        }

        private void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [SerializeField] private DialogController m_dialogController;
        public static DialogController DialogController { get { return m_instance.m_dialogController; } }

        // -----------------------
        // SCENES MANAGEMENT
        // -----------------------
        [SerializeField] private List<string> m_levelNames;
        public static List<string> LevelsNames { get { return m_instance.m_levelNames; } }

        [SerializeField] private int m_currentIndexScene = 0;
        [SerializeField] private float m_fadeTime = 1.0f;
        [SerializeField] private RawImage m_fadeBackground;
        public static void NextLevel()
        {
            m_instance.m_currentIndexScene++;
            if (m_instance.m_currentIndexScene >= LevelsNames.Count)
                m_instance.m_currentIndexScene = 0;

            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, true, true));
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Debug.Log("OnSceneLoaded: " + scene.name);
            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, false, false));
        }

        IEnumerator FadeLevel(float fadeTime, bool fadeIn, bool changeScene)
        {
            float current = 0.0f;
            float start = fadeIn ? 0f : 1f;
            float end = fadeIn ? 1f : 0f;

            while (current < fadeTime)
            {
                current += Time.deltaTime;
                if (m_fadeBackground)
                {
                    float alpha = Mathf.Lerp(start, end, current);
                    m_fadeBackground.color = new Color(m_fadeBackground.color.r, m_fadeBackground.color.g, m_fadeBackground.color.b, alpha);
                }

                yield return null;
            }

            if (changeScene)
                SceneManager.LoadScene(LevelsNames[m_instance.m_currentIndexScene], LoadSceneMode.Single);
            yield return null;
        }

        // -----------------------
        // PLAYER MANAGEMENT
        // -----------------------
        [SerializeField, EnumFlags] private PlayerTypesFlag m_playerTypesToSpawn;
        public static PlayerTypesFlag PlayerTypesToSpawn { get { return m_instance.m_playerTypesToSpawn; } }
        public static void ResetPlayerTypesToSpawn() { m_instance.m_playerTypesToSpawn = 0; }

        public static void SavePlayerType(PlayerTypesFlag playerType) { m_instance.m_playerTypesToSpawn |= playerType; }
    }
}
