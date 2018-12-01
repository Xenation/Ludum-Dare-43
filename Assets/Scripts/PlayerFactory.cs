using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD43
{
    [Serializable]
    public struct PlayerWithEnum
    {
        public PlayerTypesFlag Type;
        public GameObject Prefab;
    }

    public class PlayerFactory : MonoBehaviour
    {
        private static PlayerFactory m_instance;

        void Awake()
        {
            if (m_instance == null)
                m_instance = this;
            else if (m_instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            Init();
        }

        private void Init()
        {

        }

        [SerializeField] private List<PlayerWithEnum> m_prefabs;

        public static GameObject CreatePlayer(PlayerTypesFlag type, Vector3 position)
        {
            GameObject result = null;

            switch (type)
            {
                case PlayerTypesFlag.Leader:
                    result = Instantiate(m_instance.m_prefabs.FirstOrDefault(p => p.Type == PlayerTypesFlag.Leader).Prefab, position, Quaternion.identity);
                    result.GetComponent<CharController>().PlayerType = PlayerTypesFlag.Leader;
                    break;
                case PlayerTypesFlag.Small:
                    result = Instantiate(m_instance.m_prefabs.FirstOrDefault(p => p.Type == PlayerTypesFlag.Small).Prefab, position, Quaternion.identity);
                    result.GetComponent<CharController>().PlayerType = PlayerTypesFlag.Small;
                    break;
                case PlayerTypesFlag.HighJump:
                    result = Instantiate(m_instance.m_prefabs.FirstOrDefault(p => p.Type == PlayerTypesFlag.HighJump).Prefab, position, Quaternion.identity);
                    result.GetComponent<CharController>().PlayerType = PlayerTypesFlag.HighJump;
                    break;
                case PlayerTypesFlag.SmallJump:
                    result = Instantiate(m_instance.m_prefabs.FirstOrDefault(p => p.Type == PlayerTypesFlag.SmallJump).Prefab, position, Quaternion.identity);
                    result.GetComponent<CharController>().PlayerType = PlayerTypesFlag.SmallJump;
                    break;
                case PlayerTypesFlag.Strong:
                    result = Instantiate(m_instance.m_prefabs.FirstOrDefault(p => p.Type == PlayerTypesFlag.Strong).Prefab, position, Quaternion.identity);
                    result.GetComponent<CharController>().PlayerType = PlayerTypesFlag.Strong;
                    break;
                default:
                    break;
            }

            return result;
        }


    }
}
