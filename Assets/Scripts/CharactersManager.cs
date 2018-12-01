using System.Collections.Generic;
using UnityEngine;

namespace LD43 {
	public class CharactersManager : Singleton<CharactersManager> {

		public List<CharController> characters;
		private int activeCharacter = 0;

		private void Start() {
			characters[activeCharacter].Activate();
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
