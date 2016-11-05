using Coolest.Windows.ComInterface;
using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Meter Information Channels
	/// </summary>
	class AudioMeterInformationChannels {
		readonly IAudioMeterInformation audioMeterInformation;

		/// <summary>
		/// Metering Channel Count
		/// </summary>
		public int Count {
			get {
				int result;
				audioMeterInformation.GetMeteringChannelCount(out result);
				return result;
			}
		}

		/// <summary>
		/// Get Peak value
		/// </summary>
		/// <param name="index">Channel index</param>
		/// <returns>Peak value</returns>
		unsafe public float this[int index] {
			get {
				var peakValues = new float[Count];
				fixed (float* peaks = peakValues) {
					audioMeterInformation.GetChannelsPeakValues(peakValues.Length, (IntPtr) peaks);
					return peakValues[index];
				}
			}
		}

		internal AudioMeterInformationChannels(IAudioMeterInformation parent) {
			audioMeterInformation = parent;
		}
	}
}