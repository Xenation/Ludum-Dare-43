using UnityEngine;
using Xenon;

namespace LD43 {
	public class OutlineTester : MonoBehaviour {

		public float duration = 2f;
		public Gradient gradient;

		private SpriteRenderer sprRenderer;
		private ProcessManager processManager;
		private bool terminated = true;

		private void Awake() {
			sprRenderer = GetComponent<SpriteRenderer>();
			processManager = new ProcessManager();
		}

		private void Update() {
			if (terminated) {
				terminated = false;
				Process proc = new OutlineAnimProcess(duration, gradient, sprRenderer.material);
				proc.TerminateCallback += OnOutlineAnimEnd;
				processManager.LaunchProcess(proc);
			}
			processManager.UpdateProcesses(Time.deltaTime);
		}

		private void OnOutlineAnimEnd() {
			terminated = true;
		}

	}
}
