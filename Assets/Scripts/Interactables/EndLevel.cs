using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD43
{
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_charactersReady;

        private enum EndLevelState
        {
            NotReady = 0,
            Ready = 1,
            Quit = 2
        }

        EndLevelState m_state = EndLevelState.NotReady;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Players"))
                return;

            if (m_state < EndLevelState.Ready)
            {
                m_state = EndLevelState.Ready;
            }

            m_charactersReady.Add(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Players"))
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
                    if (Input.GetButtonDown("LeaderQuit"))
                    {
                        m_state = EndLevelState.Quit;

                        // TODO : hide the leader
                        CharactersManager.I.GetCurrentController().gameObject.SetActive(false);
                    }
                    break;
                case EndLevelState.Quit:
                    if (Input.GetButtonDown("Submit"))
                    {
                        GameManager.NextLevel();
                    }
                    break;
                default:
                    break;
            }
        }
    } 
}
