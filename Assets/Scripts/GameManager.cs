using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        }
        
        [SerializeField] private DialogController m_dialogController;
        public static DialogController DialogController { get { return m_instance.m_dialogController; } }

        // -----------------------
        // SCENES MANAGEMENT
        // -----------------------
        [SerializeField] private List<string> m_levelNames;
        public static List<string> LevelsNames { get { return m_instance.m_levelNames; } }

        [SerializeField] private int m_currentIndexScene = 0;

        public static void NextLevel()
        {
            m_instance.m_currentIndexScene++;
            if (m_instance.m_currentIndexScene >= LevelsNames.Count)
                m_instance.m_currentIndexScene = 0;

            SceneManager.LoadScene(LevelsNames[m_instance.m_currentIndexScene], LoadSceneMode.Single);
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
