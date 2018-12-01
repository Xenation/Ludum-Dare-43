using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

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

}
