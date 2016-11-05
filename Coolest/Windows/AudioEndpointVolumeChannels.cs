using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	class AudioEndpointVolumeChannels {
		readonly IAudioEndpointVolume audioEndPointVolume;
		readonly AudioEndpointVolumeChannel[] channels;

		/// <summary>
		/// Channel Count
		/// </summary>
		public int Count {
			get {
				int result;
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetChannelCount(out result));
				return result;
			}
		}

		/// <summary>
		/// Indexer - get a specific channel
		/// </summary>
		public AudioEndpointVolumeChannel this[int index] {
			get {
				return channels[index];
			}
		}

		internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent) {
			audioEndPointVolume = parent;

			int ChannelCount = Count;
			channels = new AudioEndpointVolumeChannel[ChannelCount];
			for (int i = 0; i < ChannelCount; i++) {
				channels[i] = new AudioEndpointVolumeChannel(audioEndPointVolume, i);
			}
		}
	}
}