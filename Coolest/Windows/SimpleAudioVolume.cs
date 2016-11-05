using Coolest.Windows.ComInterface;
using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	class SimpleAudioVolume : DisposableObject {
		private ISimpleAudioVolume simpleAudioVolume;

		/// <summary>
		/// Creates a new Audio endpoint volume
		/// </summary>
		/// <param name="realSimpleVolume">ISimpleAudioVolume COM interface</param>
		internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume) {
			simpleAudioVolume = realSimpleVolume;
		}

		protected override void Dispose(bool disposing) {
			ReleaseComObject(ref simpleAudioVolume);
		}

		/// <summary>
		/// Allows the user to adjust the volume from
		/// 0.0 to 1.0
		/// </summary>
		public float Volume {
			get {
				float result;
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out result));
				return result;
			}
			set {
				if ((value >= 0.0) && (value <= 1.0)) {
					Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
				}
			}
		}

		/// <summary>
		/// Mute
		/// </summary>
		public bool Mute {
			get {
				bool result;
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty));
			}
		}
	}
}