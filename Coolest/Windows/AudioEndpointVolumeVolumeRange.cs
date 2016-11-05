using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	struct AudioEndpointVolumeVolumeRange {
		readonly float volumeMinDecibels;
		readonly float volumeMaxDecibels;
		readonly float volumeIncrementDecibels;

		internal AudioEndpointVolumeVolumeRange(IAudioEndpointVolume parent) {
			parent.GetVolumeRange(out volumeMinDecibels, out volumeMaxDecibels, out volumeIncrementDecibels);
		}

		/// <summary>
		/// Minimum Decibels
		/// </summary>
		public float MinDecibels => volumeMinDecibels;

		/// <summary>
		/// Maximum Decibels
		/// </summary>
		public float MaxDecibels => volumeMaxDecibels;

		/// <summary>
		/// Increment Decibels
		/// </summary>
		public float IncrementDecibels => volumeIncrementDecibels;
	}
}