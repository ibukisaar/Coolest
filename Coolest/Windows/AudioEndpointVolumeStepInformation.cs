using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	struct AudioEndpointVolumeStepInformation {
		private readonly uint step;
		private readonly uint stepCount;

		internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent) {
			parent.GetVolumeStepInfo(out step, out stepCount);
		}

		/// <summary>
		/// Step
		/// </summary>
		public uint Step => step;

		/// <summary>
		/// StepCount
		/// </summary>
		public uint StepCount => stepCount;
	}
}