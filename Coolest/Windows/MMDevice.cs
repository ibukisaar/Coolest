using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	class MMDevice : DisposableObject {
		private IMMDevice deviceInterface;
		private PropertyStore propertyStore;
		private AudioMeterInformation audioMeterInformation;
		private AudioEndpointVolume audioEndpointVolume;
		private AudioSessionManager audioSessionManager;


		private void GetPropertyInformation() {
			IPropertyStore propstore;
			Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(StorageAccess.Read, out propstore));
			propertyStore = new PropertyStore(propstore);
		}

		private AudioClient GetAudioClient() {
			object result;
			deviceInterface.Activate(typeof(IAudioClient).GUID, ClsCtx.ALL, IntPtr.Zero, out result);
			return new AudioClient(result as IAudioClient);
		}

		private void GetAudioMeterInformation() {
			object result;
			deviceInterface.Activate(typeof(IAudioMeterInformation).GUID, ClsCtx.ALL, IntPtr.Zero, out result);
			audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
		}

		private void GetAudioEndpointVolume() {
			object result;
			deviceInterface.Activate(typeof(IAudioEndpointVolume).GUID, ClsCtx.ALL, IntPtr.Zero, out result);
			audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
		}

		private void GetAudioSessionManager() {
			object result;
			deviceInterface.Activate(typeof(IAudioSessionManager).GUID, ClsCtx.ALL, IntPtr.Zero, out result);
			audioSessionManager = new AudioSessionManager(result as IAudioSessionManager);
		}



		/// <summary>
		/// Audio Client
		/// </summary>
		public AudioClient AudioClient => GetAudioClient();

		/// <summary>
		/// Audio Meter Information
		/// </summary>
		public AudioMeterInformation AudioMeterInformation {
			get {
				if (audioMeterInformation == null)
					GetAudioMeterInformation();

				return audioMeterInformation;
			}
		}

		/// <summary>
		/// Audio Endpoint Volume
		/// </summary>
		public AudioEndpointVolume AudioEndpointVolume {
			get {
				if (audioEndpointVolume == null)
					GetAudioEndpointVolume();

				return audioEndpointVolume;
			}
		}

		/// <summary>
		/// AudioSessionManager instance
		/// </summary>
		public AudioSessionManager AudioSessionManager {
			get {
				if (audioSessionManager == null)
					GetAudioSessionManager();

				return audioSessionManager;
			}
		}

		/// <summary>
		/// Properties
		/// </summary>
		public PropertyStore Properties {
			get {
				if (propertyStore == null)
					GetPropertyInformation();

				return propertyStore;
			}
		}

		/// <summary>
		/// Friendly name for the endpoint
		/// </summary>
		public string FriendlyName {
			get {
				if (propertyStore == null) {
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName)) {
					return (string) propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
				} else
					return "Unknown";
			}
		}

		/// <summary>
		/// Friendly name of device
		/// </summary>
		public string DeviceFriendlyName {
			get {
				if (propertyStore == null) {
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName)) {
					return (string) propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
				} else {
					return "Unknown";
				}
			}
		}

		/// <summary>
		/// Icon path of device
		/// </summary>
		public string IconPath {
			get {
				if (propertyStore == null) {
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_Device_IconPath)) {
					return (string) propertyStore[PropertyKeys.PKEY_Device_IconPath].Value;
				} else
					return "Unknown";
			}
		}

		/// <summary>
		/// Device ID
		/// </summary>
		public string ID {
			get {
				string result;
				Marshal.ThrowExceptionForHR(deviceInterface.GetId(out result));
				return result;
			}
		}

		/// <summary>
		/// Data Flow
		/// </summary>
		public DataFlow DataFlow {
			get {
				DataFlow result;
				var ep = deviceInterface as IMMEndpoint;
				ep.GetDataFlow(out result);
				return result;
			}
		}

		/// <summary>
		/// Device State
		/// </summary>
		public DeviceState State {
			get {
				DeviceState result;
				Marshal.ThrowExceptionForHR(deviceInterface.GetState(out result));
				return result;
			}
		}

		public Role Role { get; }

		internal MMDevice(IMMDevice realDevice, Role role) {
			deviceInterface = realDevice;
			Role = role;
		}

		/// <summary>
		/// To string
		/// </summary>
		public override string ToString() {
			return FriendlyName;
		}

		protected override void Dispose(bool disposing) {
			ReleaseComObject(ref deviceInterface);
		}
	}
}
