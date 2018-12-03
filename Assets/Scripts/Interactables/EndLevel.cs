using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD43
{
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_charactersReady;
        [SerializeField] private AudioSource m_endLevelSource;
        [SerializeField] private GameObject m_indicator;

        private bool leaderSaved = false;

        private enum EndLevelState
        {
            NotReady = 0,
            Ready = 1,
            Quit = 2
        }

        EndLevelState m_state = EndLevelState.NotReady;

        private void Start()
        {
            if (m_indicator)
                m_indicator.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.GetComponent<CharController>())
                return;

            if (m_state < EndLevelState.Ready)
            {
                m_state = EndLevelState.Ready;
            }

            m_charactersReady.Add(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.GetComponent<CharController>())
                return;

            GameObject toDelete = m_charactersReady.FirstOrDefault(c => GameObject.ReferenceEquals(c, collision.gameObject));
            if (toDelete)
                m_charactersReady.Remove(toDelete);

            if (m_state < EndLevelState.Ready && m_charactersReady.Count == 0)
            {
                m_state = EndLevelState.NotReady;
            }
        }

        private bool m_isGoinNext = false;
        private void Update()
        {
            if (m_isGoinNext)
                return;

            m_indicator.SetActive(m_charactersReady != null && m_charactersReady.Where(c => c.activeInHierarchy).ToArray().Length > 0);

            switch (m_state)
            {
                case EndLevelState.NotReady:
                    break;
                case EndLevelState.Ready:
                    CharController currentController = CharactersManager.I.GetCurrentController();
                    if (Input.GetButtonDown("LeaderQuit") && m_charactersReady != null && m_charactersReady.Count <= 5 && m_charactersReady.FirstOrDefault(c => GameObject.ReferenceEquals(c, currentController.gameObject)))
                    {
                        GameManager.SavePlayerType(currentController.PlayerType);

                        if (currentController.PlayerType == PlayerTypesFlag.Leader)
                        {
                            leaderSaved = true;
                            GameManager.DisplayLeaderSaved();
                        }
                        currentController.gameObject.SetActive(false);

                        //if (m_endLevelSource)
                        //    m_endLevelSource.Play();

                        if (GameManager.PlayerTypesToSpawn == GameManager.PlayerHereAtStart && !m_isGoinNext) // everyone saved
                        {
                            m_isGoinNext = true;
                            GameManager.NextLevel();
                            return;
                        }

                        if (!m_isGoinNext)
                            CharactersManager.I.NextCharacter();
                    }
                    if (Input.GetButtonDown("Submit") && leaderSaved && !m_isGoinNext)
                    {
                        print("foo 2");
                        m_isGoinNext = true;
                        GameManager.NextLevel();
                    }
                    break;
                case EndLevelState.Quit:

                    break;
                default:
                    break;
            }
        }
    }
}
