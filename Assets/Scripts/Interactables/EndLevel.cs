using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private GameObject m_leader;
    private enum EndLevelState
    {
        NotReady = 0,
        Ready = 1,
        Quit = 2
    }

    EndLevelState m_state = EndLevelState.NotReady;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_state < EndLevelState.Ready)
        {
            m_state = EndLevelState.Ready;
            m_leader = collision.gameObject;
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_state < EndLevelState.Ready)
        {
            m_state = EndLevelState.NotReady;
            m_leader = null;
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
                    m_leader.gameObject.SetActive(false);
                }
                break;
            case EndLevelState.Quit:
                if(Input.GetButtonDown("ChangeLevel"))
                {
                    GameManager.NextLevel();
                }
                break;
            default:
                break;
        }
    }
}
