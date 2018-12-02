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
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            m_instance.Init();
        }

        private void Init()
        {
            m_currentPlayerIndicator = Instantiate(m_playerIndicatorPrefab);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [SerializeField] private DialogController m_dialogController;
        public static DialogController DialogController { get { return m_instance.m_dialogController; } }

        IEnumerator DialogAfterSeconds(float waitingTime = 0.0f)
        {
            yield return new WaitForSeconds(waitingTime);
            m_instance.m_dialogController.NextText();
        }

        // -----------------------
        // SCENES MANAGEMENT
        // -----------------------
        [SerializeField] private List<string> m_levelNames;
        public static List<string> LevelsNames { get { return m_instance.m_levelNames; } }

        [SerializeField] private int m_currentIndexScene = 0;
        [SerializeField] private float m_fadeTime = 1.0f;
        [SerializeField] private RawImage m_fadeBackground;
        public static void NextLevel(int nextLevelIndex = -1)
        {
            UpdatePlayerIndicator(null, 0f, false);

            if (nextLevelIndex < 0)
                m_instance.m_currentIndexScene++;
            else
                m_instance.m_currentIndexScene = nextLevelIndex;

            if (m_instance.m_currentIndexScene >= LevelsNames.Count)
                m_instance.m_currentIndexScene = 0;

            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, true, true));
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Debug.Log("OnSceneLoaded: " + scene.name);
            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, false, false));
            m_instance.StartCoroutine(m_instance.DialogAfterSeconds(1.0f));
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

        [SerializeField] private GameObject m_playerIndicatorPrefab;
        [SerializeField] private Vector3 m_playerIndicatorOffset = new Vector3(0f, 0f, 0f);
        private GameObject m_currentPlayerIndicator;
        public static GameObject PlayerIndicatorPrefab { get { return m_instance.m_playerIndicatorPrefab; } }
        public static void UpdatePlayerIndicator(CharController parent, float yOffset, bool display = true)
        {
            if(!m_instance.m_currentPlayerIndicator)
                m_instance.m_currentPlayerIndicator = Instantiate(m_instance.m_playerIndicatorPrefab);

            if (parent)
            {
                m_instance.m_currentPlayerIndicator.transform.parent = parent.OverlayPosition.transform;
                m_instance.m_currentPlayerIndicator.transform.localPosition = m_instance.m_playerIndicatorOffset;
				m_instance.m_currentPlayerIndicator.transform.rotation = Quaternion.identity;
            }

            m_instance.m_currentPlayerIndicator.SetActive(display);
        }

        [SerializeField, EnumFlags] private PlayerTypesFlag m_playerTypesToSpawn;
        public static PlayerTypesFlag PlayerTypesToSpawn { get { return m_instance.m_playerTypesToSpawn; } }
        public static void ResetPlayerTypesToSpawn() { m_instance.m_playerTypesToSpawn = 0; }

        public static void SavePlayerType(PlayerTypesFlag playerType) { m_instance.m_playerTypesToSpawn |= playerType; }
    }
}
