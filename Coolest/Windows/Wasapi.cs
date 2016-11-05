using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Coolest.Windows.ComInterface;

namespace Coolest.Windows {
	sealed class Wasapi : DisposableObject {
		private struct FormatKey : IEquatable<FormatKey> {
			public ShareMode ShareMode;
			public WaveFormat Format;

			public override int GetHashCode() {
				return Format.GetHashCode() ^ ShareMode.GetHashCode();
			}

			public bool Equals(FormatKey other) {
				return ShareMode == other.ShareMode && Format.Equals(other.Format);
			}
		}

		static readonly IList<int> sampleRatesTemplate = new int[] { 44100, 48000, 96000, 192000 };
		static readonly IList<int> bitsTemplate = new int[] { 8, 16, 24, 32 };
		static readonly IList<int> channelsTemplate = new int[] { 1, 2 };
		static readonly HashSet<FormatKey> supportedFormats = new HashSet<FormatKey>();


		private AudioClient audioClient;
		private MMDevice device;
		private ShareMode shareMode;
		private bool eventSync;
		private int durationMillisecond;
		private WaveFormat format;
		private StreamFlags streamFlags;

		public AudioClient AudioClient => audioClient;
		public MMDevice Device => device;
		public ShareMode ShareMode => shareMode;
		public bool EventSync => eventSync;
		public int DurationMillisecond => durationMillisecond;
		public WaveFormat Format => format;
		public StreamFlags StreamFlags => streamFlags;

		public Wasapi(MMDevice device, StreamFlags streamFlags, ShareMode shareMode, bool eventSync, int durationMillisecond) {
			audioClient = device.AudioClient;
			this.streamFlags = streamFlags;
			this.device = device;
			this.shareMode = shareMode;
			this.eventSync = eventSync;
			this.durationMillisecond = durationMillisecond;
		}

		public void Initialize(WaveFormat format) {
			if (shareMode == ShareMode.Exclusive) {
				eventSync = true;
			}

			if (eventSync) streamFlags |= StreamFlags.StreamFlagsEventCallback;

			this.format = InitializeMatchFormat(format);
			if (this.format == null) {
				throw new ArgumentException("无法从设备中找到支持的格式");
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				audioClient.Dispose();
				device.Dispose();
			}
		}

		private WaveFormat TryInitialize(WaveFormat inFormat) {
			WaveFormatExtensible result;
			bool success = audioClient.IsFormatSupported(shareMode, inFormat, out result);
			if (success) {
				if (result != null) {
					inFormat = result;
				}

				long bufferDuration = durationMillisecond * 10000L;
				long periodicity = shareMode == ShareMode.Shared ? 0 : bufferDuration;
				int hret = audioClient.Initialize(shareMode, streamFlags, bufferDuration, periodicity, inFormat, Guid.Empty);
				Marshal.ThrowExceptionForHR(hret);
				return inFormat;
			}
			return null;
		}

		private WaveFormat InitializeMatchFormat(int ratePointer, int bitPointer, int channelPointer) {
			int sampleRate = sampleRatesTemplate[ratePointer];
			int bits = bitsTemplate[bitPointer];
			int channels = channelsTemplate[channelPointer];

			WaveFormat temp, result;

			if (bits == 24) {
				temp = new WaveFormatExtensible(sampleRate, 32, channels, AudioSubTypes.Pcm, 24);
				result = TryInitialize(temp);
				if (result != null) return result;
			}

			temp = new WaveFormatExtensible(sampleRate, bits, channels);
			result = TryInitialize(temp);
			if (result != null) return result;

			return null;
		}

		private WaveFormat InitializeMatchFormat(WaveFormat inFormat) {
			WaveFormat result = null;

			if (supportedFormats.Contains(new FormatKey { Format = inFormat, ShareMode = shareMode })) {
				return inFormat;
			}

			int rateIndex = sampleRatesTemplate.IndexOf(inFormat.SampleRate);
			int bitIndex = bitsTemplate.IndexOf(inFormat.ValidBitsPerSample);
			int channelIndex = channelsTemplate.IndexOf(inFormat.Channels);

			for (int channelPointer = channelIndex; channelPointer < channelsTemplate.Count; channelPointer++) {
				for (int ratePointer = rateIndex; ratePointer < sampleRatesTemplate.Count; ratePointer++) {
					for (int bitPointer = bitIndex; bitPointer < bitsTemplate.Count; bitPointer++) {
						result = InitializeMatchFormat(ratePointer, bitPointer, channelPointer);
						if (result != null) goto Success;
					}
				}
			}

			for (int channelPointer = channelIndex; channelPointer >= 0; channelPointer--) {
				for (int ratePointer = rateIndex; ratePointer >= 0; ratePointer--) {
					int bitPointer =
						ratePointer == rateIndex && channelPointer == channelIndex ? bitIndex + 1 : bitIndex;
					for (; bitPointer >= 0; bitPointer--) {
						result = InitializeMatchFormat(ratePointer, bitPointer, channelPointer);
						if (result != null) goto Success;
					}
				}
			}

			return null;
			Success:
			supportedFormats.Add(new FormatKey { Format = result, ShareMode = shareMode });
			return result;
		}

		public static MMDevice GetDefaultDevice(DataFlow dataFlow, Role role = Role.Multimedia) {
			IMMDeviceEnumerator deviceEnumerator = null;
			IMMDevice device;
			try {
				deviceEnumerator = (IMMDeviceEnumerator) new MMDeviceEnumerator();
				deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out device);
				return new MMDevice(device, role);
			} finally {
				ReleaseComObject(ref deviceEnumerator);
			}
		}
	}
}
