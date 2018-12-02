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
        private bool leaderSaved = false;

        private enum EndLevelState
        {
            NotReady = 0,
            Ready = 1,
            Quit = 2
        }

        EndLevelState m_state = EndLevelState.NotReady;

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

        private void Update()
        {
            switch (m_state)
            {
                case EndLevelState.NotReady:
                    break;
                case EndLevelState.Ready:
                    CharController currentController = CharactersManager.I.GetCurrentController();
                    if (Input.GetButtonDown("LeaderQuit") && m_charactersReady.FirstOrDefault(c => GameObject.ReferenceEquals(c, currentController.gameObject)))
                    {
                        GameManager.SavePlayerType(currentController.PlayerType);

                        if (currentController.PlayerType == PlayerTypesFlag.Leader)
                            leaderSaved = true;

                        currentController.gameObject.SetActive(false);

                        if (m_endLevelSource)
                            m_endLevelSource.Play();
                    }
                    if (Input.GetButtonDown("Submit") && leaderSaved)
                    {

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
