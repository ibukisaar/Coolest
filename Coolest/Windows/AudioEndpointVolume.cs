using Coolest.Windows.ComInterface;
using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Endpoint Volume
	/// </summary>
	class AudioEndpointVolume : DisposableObject {
		private readonly IAudioEndpointVolume audioEndPointVolume;
		private readonly AudioEndpointVolumeChannels channels;
		private readonly AudioEndpointVolumeStepInformation stepInformation;
		private readonly AudioEndpointVolumeVolumeRange volumeRange;
		private readonly EEndpointHardwareSupport hardwareSupport;
		private AudioEndpointVolumeCallback callBack;

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

		/// <summary>
		/// On Volume Notification
		/// </summary>
		public event Action<AudioVolumeNotificationData> OnVolumeNotification;

		/// <summary>
		/// Volume Range
		/// </summary>
		public AudioEndpointVolumeVolumeRange VolumeRange {
			get {
				return volumeRange;
			}
		}

		/// <summary>
		/// Hardware Support
		/// </summary>
		public EEndpointHardwareSupport HardwareSupport {
			get {
				return hardwareSupport;
			}
		}

		/// <summary>
		/// Step Information
		/// </summary>
		public AudioEndpointVolumeStepInformation StepInformation {
			get {
				return stepInformation;
			}
		}

		/// <summary>
		/// Channels
		/// </summary>
		public AudioEndpointVolumeChannels Channels {
			get {
				return channels;
			}
		}

		/// <summary>
		/// Master Volume Level
		/// </summary>
		public float MasterVolumeLevel {
			get {
				float result;
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevel(out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevel(value, ref notificationGuid));
			}
		}

		/// <summary>
		/// Master Volume Level Scalar
		/// </summary>
		public float MasterVolumeLevelScalar {
			get {
				float result;
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevelScalar(out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevelScalar(value, ref notificationGuid));
			}
		}

		/// <summary>
		/// Mute
		/// </summary>
		public bool Mute {
			get {
				bool result;
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMute(out result));
				return result;
			}
			set {
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMute(value, ref notificationGuid));
			}
		}

		/// <summary>
		/// Volume Step Up
		/// </summary>
		public void VolumeStepUp() {
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepUp(ref notificationGuid));
		}

		/// <summary>
		/// Volume Step Down
		/// </summary>
		public void VolumeStepDown() {
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepDown(ref notificationGuid));
		}

		/// <summary>
		/// Creates a new Audio endpoint volume
		/// </summary>
		/// <param name="realEndpointVolume">IAudioEndpointVolume COM interface</param>
		internal AudioEndpointVolume(IAudioEndpointVolume realEndpointVolume) {
			uint hardwareSupp;

			audioEndPointVolume = realEndpointVolume;
			channels = new AudioEndpointVolumeChannels(audioEndPointVolume);
			stepInformation = new AudioEndpointVolumeStepInformation(audioEndPointVolume);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.QueryHardwareSupport(out hardwareSupp));
			hardwareSupport = (EEndpointHardwareSupport) hardwareSupp;
			volumeRange = new AudioEndpointVolumeVolumeRange(audioEndPointVolume);
			callBack = new AudioEndpointVolumeCallback(this);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.RegisterControlChangeNotify(callBack));
		}

		internal void FireNotification(AudioVolumeNotificationData notificationData) {
			OnVolumeNotification?.Invoke(notificationData);
		}
		#region IDisposable Members

		protected override void Dispose(bool disposing) {
			if (callBack != null) {
				audioEndPointVolume.UnregisterControlChangeNotify(callBack);
				callBack = null;
			}
		}

		#endregion

	}
}