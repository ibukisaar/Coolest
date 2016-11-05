using Coolest.Windows.ComInterface;
using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Endpoint Volume Channel
	/// </summary>
	class AudioEndpointVolumeChannel {
		private readonly uint channel;
		private readonly IAudioEndpointVolume audioEndpointVolume;

		private Guid notificationGuid = Guid.Empty;

		/// <summary>
		/// GUID to pass to AudioEndpointVolumeCallback
		/// </summary>
		public Guid NotificationGuid {
			get {
				return notificationGuid;
			}
			set {
				notificationGuid = value;
			}
		}

		internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel) {
			this.channel = (uint) channel;
			audioEndpointVolume = parent;
		}

		/// <summary>
		/// Volume Level
		/// </summary>
		public float VolumeLevel {
			get {
				float result;
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, ref notificationGuid));
			}
		}

		/// <summary>
		/// Volume Level Scalar
		/// </summary>
		public float VolumeLevelScalar {
			get {
				float result;
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, ref notificationGuid));
			}
		}

	}
}