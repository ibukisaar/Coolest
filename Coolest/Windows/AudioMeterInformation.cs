using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Coolest.Windows.ComInterface;

namespace Coolest.Windows {
	class AudioMeterInformation {
		private readonly IAudioMeterInformation audioMeterInformation;
		private readonly EEndpointHardwareSupport hardwareSupport;
		private readonly AudioMeterInformationChannels channels;

		internal AudioMeterInformation(IAudioMeterInformation realInterface) {
			int hardwareSupp;

			audioMeterInformation = realInterface;
			Marshal.ThrowExceptionForHR(audioMeterInformation.QueryHardwareSupport(out hardwareSupp));
			hardwareSupport = (EEndpointHardwareSupport) hardwareSupp;
			channels = new AudioMeterInformationChannels(audioMeterInformation);

		}

		/// <summary>
		/// Peak Values
		/// </summary>
		public AudioMeterInformationChannels PeakValues => channels;

		/// <summary>
		/// Hardware Support
		/// </summary>
		public EEndpointHardwareSupport HardwareSupport => hardwareSupport;

		/// <summary>
		/// Master Peak Value
		/// </summary>
		public float MasterPeakValue {
			get {
				float result;
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetPeakValue(out result));
				return result;
			}
		}
	}
}
