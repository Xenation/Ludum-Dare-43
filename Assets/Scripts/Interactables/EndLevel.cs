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

                        StartCoroutine(FadePerso(currentController.gameObject, 1.0f));

                        if (m_endLevelSource)
                            m_endLevelSource.Play();
                        
                        if ((GameManager.PlayerTypesToSpawn == GameManager.PlayerHereAtStart || CharactersManager.I.characters.Count == GameManager.NbPlayerTypesToSpawn) && !m_isGoinNext) // everyone saved
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
                        //print("foo 2");
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
        IEnumerator FadePerso(GameObject target, float fadeTime = 0.5f)
        {
            float current = 0.0f;
            float start = 1f;
            float end = 0f;

            Vector3 startPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            Vector3 endPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 0.5f);

            SpriteRenderer rend = target.GetComponentInChildren<SpriteRenderer>();

            while (current < fadeTime)
            {
                current += Time.deltaTime;
                if (target && rend)
                {
                    float alpha = Mathf.Lerp(start, end, current / fadeTime);
                    rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
                    target.transform.position = Vector3.Lerp(startPos, endPos, current / fadeTime);
                }

                yield return null;
            }

            target.SetActive(false);
            yield return null;
        }
    }
}
