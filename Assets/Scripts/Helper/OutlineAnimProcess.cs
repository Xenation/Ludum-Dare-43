using UnityEngine;
using Xenon;
using Xenon.Processes;

namespace LD43 {
	public class OutlineAnimProcess : TimedProcess {

		private Gradient colorGradient;
		private Material material;

		public OutlineAnimProcess(float duration, Gradient gradient, Material mat) : base(duration) {
			colorGradient = gradient;
			material = mat;
		}

		public override void OnBegin() {
			base.OnBegin();
		}

		public override void TimeUpdated() {
			material.SetColor("_OutlineColor", colorGradient.Evaluate(TimePortion));
			base.TimeUpdated();
		}

		public override void Update(float dt) {
			base.Update(dt);
		}

		public override void OnTerminate() {
			base.OnTerminate();
		}

	}
}
