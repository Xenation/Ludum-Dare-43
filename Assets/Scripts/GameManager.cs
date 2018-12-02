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

            if (m_instance.m_menuMusic)
                m_instance.m_menuMusic.Play();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (m_currentIndexScene > 0 && m_currentIndexScene < m_levelNames.Count && Input.GetButtonDown("Reset"))
            {
                ResetLevel();
            }
        }

        public static void ResetLevel()
        {
            // reset
            m_instance.m_playerTypesToSpawn = m_instance.m_playersHereAtStart;
            NextLevel(m_instance.m_currentIndexScene);
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

        [Header("Scenes")]
        [SerializeField] private List<string> m_levelNames;
        public static List<string> LevelsNames { get { return m_instance.m_levelNames; } }

        [SerializeField] private int m_currentIndexScene = 0;
        [SerializeField] private float m_fadeTime = 1.0f;
        [SerializeField] private RawImage m_fadeBackground;

        private string m_lastSceneName;
        private string m_currentSceneName;

        public static void NextLevel(int nextLevelIndex = -1)
        {
            UpdatePlayerIndicator(null, 0f, false);
            int lastIndex = m_instance.m_currentIndexScene;

            if (nextLevelIndex < 0)
                m_instance.m_currentIndexScene++;
            else
                m_instance.m_currentIndexScene = nextLevelIndex;

            if (m_instance.m_currentIndexScene >= LevelsNames.Count)
                m_instance.m_currentIndexScene = 0;

            if (m_instance.m_currentIndexScene == 0) // => menu
            {
                if (m_instance.m_menuMusic)
                    m_instance.m_menuMusic.Play();

                m_instance.m_wantToStopGameMusic = true;
                if (m_instance.m_introMusic)
                    SoundHelper.I.StopWithFade(m_instance.m_introMusic, 0.0f, 1.0f);
                if (m_instance.m_loopMusic)
                    SoundHelper.I.StopWithFade(m_instance.m_loopMusic, 0.0f, 1.0f);
            }
            else if (m_instance.m_currentIndexScene == 1)
            {
                if (!m_instance.m_gameMusicRunning && lastIndex == 0)
                {
                    if (m_instance.m_menuMusic)
                        SoundHelper.I.StopWithFade(m_instance.m_menuMusic, 0.0f, 1.0f);

                    m_instance.StartCoroutine(m_instance.PlayGameMusic());
                }
            }

            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, true, true));
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_currentSceneName = scene.name;
            //Debug.Log("OnSceneLoaded: " + scene.name);
            m_instance.StartCoroutine(m_instance.FadeLevel(m_instance.m_fadeTime, false, false));

            if (m_currentSceneName != m_lastSceneName)
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
            {
                m_lastSceneName = m_currentSceneName;
                SceneManager.LoadScene(LevelsNames[m_instance.m_currentIndexScene], LoadSceneMode.Single);
            }
            yield return null;
        }

        [Header("Music")]
        [SerializeField] private AudioSource m_menuMusic;
        [SerializeField] private AudioSource m_introMusic;
        [SerializeField] private AudioSource m_loopMusic;

        private bool m_wantToStopGameMusic = false;
        private bool m_gameMusicRunning = false;

        IEnumerator PlayGameMusic()
        {
            m_gameMusicRunning = true;
            if (m_introMusic)
            {
                m_introMusic.Play();

                yield return null;

                while (m_introMusic.isPlaying)
                {
                    if (m_wantToStopGameMusic)
                    {
                        m_wantToStopGameMusic = false;
                        m_gameMusicRunning = false;
                        yield break;
                    }

                    yield return null;
                }
            }

            m_gameMusicRunning = false;
            m_wantToStopGameMusic = false;

            if (m_loopMusic)
                m_loopMusic.Play();
        }

        // -----------------------
        // PLAYER MANAGEMENT
        // -----------------------
        [Header("Player")]
        [SerializeField] private GameObject m_playerIndicatorPrefab;
        [SerializeField] private Vector3 m_playerIndicatorOffset = new Vector3(0f, 0f, 0f);
        private GameObject m_currentPlayerIndicator;
        public static GameObject PlayerIndicatorPrefab { get { return m_instance.m_playerIndicatorPrefab; } }
        public static void UpdatePlayerIndicator(CharController parent, float yOffset, bool display = true)
        {
            if (!m_instance.m_currentPlayerIndicator)
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
        private PlayerTypesFlag m_playersHereAtStart;
        public static PlayerTypesFlag PlayerTypesToSpawn { get { return m_instance.m_playerTypesToSpawn; } }
        public static PlayerTypesFlag PlayerHereAtStart { get { return m_instance.m_playersHereAtStart; } }
        public static void ResetPlayerTypesToSpawn()
        {
            m_instance.m_playersHereAtStart = m_instance.m_playerTypesToSpawn;
            m_instance.m_playerTypesToSpawn = 0;
        }

        public static void SavePlayerType(PlayerTypesFlag playerType) { m_instance.m_playerTypesToSpawn |= playerType; }
    }
}
