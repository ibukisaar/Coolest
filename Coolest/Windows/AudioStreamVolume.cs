using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	class AudioStreamVolume : DisposableObject {
		IAudioStreamVolume audioStreamVolumeInterface;

		internal AudioStreamVolume(IAudioStreamVolume audioStreamVolumeInterface) {
			this.audioStreamVolumeInterface = audioStreamVolumeInterface;
		}

		/// <summary>
		/// Verify that the channel index is valid.
		/// </summary>
		/// <param name="channelIndex"></param>
		/// <param name="parameter"></param>
		private void CheckChannelIndex(int channelIndex, string parameter) {
			int channelCount = ChannelCount;
			if (channelIndex >= channelCount) {
				throw new ArgumentOutOfRangeException(parameter, "You must supply a valid channel index < current count of channels: " + channelCount.ToString());
			}
		}

		/// <summary>
		/// Return the current stream volumes for all channels
		/// </summary>
		/// <returns>An array of volume levels between 0.0 and 1.0 for each channel in the audio stream.</returns>
		public float[] GetAllVolumes() {
			uint channels;
			float[] levels;
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out channels));
			levels = new float[channels];
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetAllVolumes(channels, levels));
			return levels;
		}

		/// <summary>
		/// Returns the current number of channels in this audio stream.
		/// </summary>
		public int ChannelCount {
			get {
				uint channels;
				Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out channels));
				return unchecked((int) channels);
			}
		}

		/// <summary>
		/// Return the current volume for the requested channel.
		/// </summary>
		/// <param name="channelIndex">The 0 based index into the channels.</param>
		/// <returns>The volume level for the channel between 0.0 and 1.0.</returns>
		public float GetChannelVolume(int channelIndex) {
			CheckChannelIndex(channelIndex, nameof(channelIndex));

			uint index = unchecked((uint) channelIndex);
			float level;
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelVolume(index, out level));
			return level;
		}

		/// <summary>
		/// Set the volume level for each channel of the audio stream.
		/// </summary>
		/// <param name="levels">An array of volume levels (between 0.0 and 1.0) one for each channel.</param>
		/// <remarks>
		/// A volume level MUST be supplied for reach channel in the audio stream.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="levels"/> does not contain <see cref="ChannelCount"/> elements.
		/// </exception>
		public void SetAllVolumes(float[] levels) {
			// Make friendly Net exceptions for common problems:
			int channelCount = ChannelCount;
			if (levels == null) {
				throw new ArgumentNullException(nameof(levels));
			}
			if (levels.Length != channelCount) {
				throw new ArgumentOutOfRangeException(
					nameof(levels),
					string.Format("SetAllVolumes MUST be supplied with a volume level for ALL channels. The AudioStream has {0} channels and you supplied {1} channels.",
								  channelCount, levels.Length));
			}
			for (int i = 0; i < levels.Length; i++) {
				float level = levels[i];
				if (level < 0.0f) throw new ArgumentOutOfRangeException(nameof(levels), "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i.ToString());
				if (level > 1.0f) throw new ArgumentOutOfRangeException(nameof(levels), "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i.ToString());
			}
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetAllVoumes(unchecked((uint) channelCount), levels));
		}

		/// <summary>
		/// Sets the volume level for one channel in the audio stream.
		/// </summary>
		/// <param name="index">The 0-based index into the channels to adjust the volume of.</param>
		/// <param name="level">The volume level between 0.0 and 1.0 for this channel of the audio stream.</param>
		public void SetChannelVolume(int index, float level) {
			CheckChannelIndex(index, nameof(index));

			if (level < 0.0f) throw new ArgumentOutOfRangeException(nameof(level), "Volume must be between 0.0 and 1.0");
			if (level > 1.0f) throw new ArgumentOutOfRangeException(nameof(level), "Volume must be between 0.0 and 1.0");

			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetChannelVolume(unchecked((uint) index), level));
		}

		protected override void Dispose(bool disposing) {
			if (audioStreamVolumeInterface != null) {
				ReleaseComObject(audioStreamVolumeInterface);
				audioStreamVolumeInterface = null;
			}
		}
	}
}
