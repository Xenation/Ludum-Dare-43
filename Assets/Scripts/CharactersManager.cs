using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD43 {
    //public enum PlayerTypesEnum
    //{
    //    Leader = 1,
    //    Small = 2,
    //    HighJump = 3,
    //    SmallJump = 4,
    //    Strong = 5
    //}

    [Flags]
    public enum PlayerTypesFlag
    {
        Leader = 1,
        Small = 2,
        HighJump = 4,
        SmallJump = 8,
        Strong = 16
    }

    public class CharactersManager : Singleton<CharactersManager> {
        
		public List<CharController> characters;

        [SerializeField] private List<Transform> m_startPositions = new List<Transform>();
        private int activeCharacter = 0;


        private void Start() {
            Init();
			characters[activeCharacter].Activate();
		}
        
        private void Init()
        {
            characters = new List<CharController>();
            if ((GameManager.PlayerTypesToSpawn & PlayerTypesFlag.Leader) == PlayerTypesFlag.Leader)
            {
                characters.Add(PlayerFactory.CreatePlayer(PlayerTypesFlag.Leader, m_startPositions[0].position).GetComponent<CharController>());
            }
            if ((GameManager.PlayerTypesToSpawn & PlayerTypesFlag.Small) == PlayerTypesFlag.Small)
            {
                characters.Add(PlayerFactory.CreatePlayer(PlayerTypesFlag.Small, m_startPositions[1].position).GetComponent<CharController>());
            }
            if ((GameManager.PlayerTypesToSpawn & PlayerTypesFlag.HighJump) == PlayerTypesFlag.HighJump)
            {
                characters.Add(PlayerFactory.CreatePlayer(PlayerTypesFlag.HighJump, m_startPositions[2].position).GetComponent<CharController>());
            }
            if ((GameManager.PlayerTypesToSpawn & PlayerTypesFlag.SmallJump) == PlayerTypesFlag.SmallJump)
            {
                characters.Add(PlayerFactory.CreatePlayer(PlayerTypesFlag.SmallJump, m_startPositions[3].position).GetComponent<CharController>());
            }
            if ((GameManager.PlayerTypesToSpawn & PlayerTypesFlag.Strong) == PlayerTypesFlag.Strong)
            {
                characters.Add(PlayerFactory.CreatePlayer(PlayerTypesFlag.Strong, m_startPositions[4].position).GetComponent<CharController>());
            }

            GameManager.ResetPlayerTypesToSpawn(); // set types available to 0
        }

        private void Update() {
			if (characters.Count > 1) {
				if (Input.GetButtonDown("NextCharacter")) {
					NextCharacter();
				}
			}
		}

		public void RemoveCharacter(CharController contr) {
			if (!characters.Contains(contr)) return;
			int remIndex = characters.IndexOf(contr);
			characters.Remove(contr);
			contr.Desactivate();
			if (remIndex < activeCharacter) {
				activeCharacter--;
			}
			if (characters.Count > 0) {
				activeCharacter = activeCharacter % characters.Count;
				characters[activeCharacter].Activate();
			}
		}

		public void NextCharacter() {
			characters[activeCharacter].Desactivate();
			activeCharacter++;
			activeCharacter = activeCharacter % characters.Count;
			characters[activeCharacter].Activate();
		}

        public CharController GetCurrentController()
        {
            return characters[activeCharacter];
        }
        
	}
}
