using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using Coolest.SoundOut.Windows.ComInterface;

namespace Coolest.SoundOut.Windows {
	unsafe public sealed class WasapiOut : AudioPlayerBase {
		#region Native Methods

		const string DllName = "coolest.win32.dll";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CreateAudioPlayer(out IntPtr audioPlayerPtr, ShareMode shareMode, long hnsRequestDuration, Role role);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ReleaseAudioPlayer(IntPtr audioPlayerPtr);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint AudioPlayerInitialize(IntPtr audioPlayerPtr, WaveFormat format);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static extern uint AudioPlayerRequestFrameCount(IntPtr audioPlayerPtr, out int framesCount, bool first);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static extern uint AudioPlayerSubmitBuffer(IntPtr audioPlayerPtr, byte[] buffer, int framesCount);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint AudioPlayerStart(IntPtr audioPlayerPtr);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint AudioPlayerStop(IntPtr audioPlayerPtr);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint AudioPlayerSetEventHandle(IntPtr audioPlayerPtr, IntPtr eventHandle);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static extern uint AudioPlayerIsFormatSupported(IntPtr audioPlayerPtr, WaveFormat waveFormat, IntPtr closestMatch);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static extern uint AudioPlayerIsFormatSupported(IntPtr audioPlayerPtr, WaveFormat waveFormat, void* @null = null);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static extern uint AudioPlayerGetMixFormat(IntPtr audioPlayerPtr, out WaveFormat format);

		#endregion

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

		[ComImport]
		[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
		private class MMDeviceEnumerator {
		}

		static readonly IList<int> sampleRatesTemplate = new int[] { 44100, 48000, 96000, 192000 };
		static readonly IList<int> bitsTemplate = new int[] { 8, 16, 24, 32 };
		static readonly IList<int> channelsTemplate = new int[] { 1, 2 };
		static readonly HashSet<FormatKey> supportedFormats = new HashSet<FormatKey>();

		const AudioClientStreamFlags DefaultStreamFlags = AudioClientStreamFlags.StreamFlagsEventCallback;

		private IAudioClient audioClient;
		private IAudioRenderClient renderClient;
		private WaveFormat format;
		private AutoResetEvent eventObject = new AutoResetEvent(false);
		private long durationMillisecond;
		private int bufferFrameCount;

		public ShareMode ShareMode { get; }
		public Role Role { get; }

		public override WaveFormat OutFormat => format;

		/// <summary>
		/// WASAPI音频输出
		/// </summary>
		/// <param name="waveStream">指定音频源</param>
		/// <param name="shareMode">指定共享模式或独占模式</param>
		/// <param name="durationMillisecond">缓冲区周期时间</param>
		/// <param name="role">指定应用场景</param>
		public WasapiOut(IWaveStream waveStream, ShareMode shareMode, int durationMillisecond = 100, Role role = Role.Multimedia)
			: base(waveStream) {

			try {
				this.durationMillisecond = durationMillisecond;
				ShareMode = shareMode;
				Role = role;

				audioClient = CreateAudioClient(role);

				var format = waveStream.Format;
				if (format.BitsPerSample == 24) {
					format = new WaveFormatExtensible(format.SampleRate, 32, format.Channels, AudioSubTypes.Pcm, 24);
				} else {
					format = new WaveFormatExtensible(format.SampleRate, format.BitsPerSample, format.Channels);
				}

				//this.format = format;
				//WaveFormat outFormat = GetSupportedFormat(format);
				//error = AudioPlayerInitialize(playerPtr, format);
				//if (error != 0) {
				//	outFormat = GetSupportedFormat(format);
				//	if (outFormat == null) throw new ArgumentException("无法从设备中找到支持的格式", nameof(waveStream));

				//	error = AudioPlayerInitialize(playerPtr, outFormat);
				//	if (error != 0) throw new WasapiOutExcpetion(error);
				//}

				//var res = IsSupportedFormat(new WaveFormatExtensible(48000, 24, format.Channels, AudioSubTypes.Pcm, 24));
				//res = IsSupportedFormat(new WaveFormat(format.SampleRate, 24, format.Channels));

				this.format = Initialize(format);
				if (this.format == null) throw new ArgumentException("无法从设备中找到支持的格式", nameof(waveStream));
				if (!this.format.Equals(format)) {
					waveStream.Resample(this.format);
				}

				audioClient.GetBufferSize(out bufferFrameCount);
				renderClient = CreateRenderClient(audioClient);
				audioClient.SetEventHandle(eventObject.SafeWaitHandle.DangerousGetHandle());
			} catch {
				Dispose();
				throw;
			}
		}

		/// <summary>
		/// WASAPI音频输出，默认使用请求周期100ms，共享模式，音乐场景
		/// </summary>
		/// <param name="waveStream">指定音频源</param>
		public WasapiOut(IWaveStream waveStream)
			: this(waveStream, ShareMode.Shared) {

		}

		private static IAudioClient CreateAudioClient(Role role) {
			IMMDeviceEnumerator deviceEnumer = null;
			IMMDevice device = null;
			try {
				IUnknown client;
				deviceEnumer = (IMMDeviceEnumerator) new MMDeviceEnumerator();
				deviceEnumer.GetDefaultAudioEndpoint(DataFlow.Render, role, out device);
				device.Activate(typeof(IAudioClient).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out client);
				return client as IAudioClient;
			} finally {
				device?.Release();
			}
		}

		private static IAudioRenderClient CreateRenderClient(IAudioClient audioClient) {
			IUnknown client;
			audioClient.GetService(typeof(IAudioRenderClient).GUID, out client);
			return client as IAudioRenderClient;
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				eventObject.Dispose();
			}

			audioClient?.Release();
			audioClient = null;
			renderClient?.Release();
			renderClient = null;

			base.Dispose(disposing);
		}

		protected override void OnPlay() {
			audioClient.Start();
		}

		protected override void OnStop() {
			audioClient.Stop();
		}

		protected override int RequestBufferLength(bool first) {
			if (!first) {
				if (!eventObject.WaitOne(1000)) return 0;
			}

			if (ShareMode == ShareMode.Shared && !first) {
				int padding;
				audioClient.GetCurrentPadding(out padding);
				return (bufferFrameCount - padding) * format.BlockAlign;
			} else {
				return bufferFrameCount * format.BlockAlign;
			}
		}

		protected override void WriteBuffer(byte[] buffer, int writeLength) {
			int framesCount = writeLength / format.BlockAlign;
			if (framesCount > 0) {
				IntPtr bufferPtr;
				renderClient.GetBuffer(framesCount, out bufferPtr);
				Marshal.Copy(buffer, 0, bufferPtr, writeLength);
				renderClient.ReleaseBuffer(framesCount, 0);
			}
		}

		//public IEnumerable<WaveFormat> GetSupportedFormats() {
		//	byte[] bits = { 24, 16, 8 };
		//	byte[] channels = { 2, 1 };

		//	foreach (var channel in channels) {
		//		WaveFormatExtensible tempFormat = new WaveFormatExtensible(format.SampleRate, 32, channel, AudioSubTypes.IeeeFloat);
		//		WaveFormat supportedFormat = IsSupportedFormat(tempFormat);
		//		if (supportedFormat != null) yield return supportedFormat;

		//		foreach (var bit in bits) {
		//			tempFormat = new WaveFormatExtensible(format.SampleRate, bit, channel, AudioSubTypes.Pcm);
		//			supportedFormat = IsSupportedFormat(tempFormat);
		//			if (supportedFormat != null) yield return supportedFormat;
		//		}
		//	}
		//}

		//public WaveFormat IsSupportedFormat(WaveFormat format) {
		//	if (ShareMode == ShareMode.Shared) {
		//		IntPtr closestMatchPtr = IntPtr.Zero;
		//		uint ret = AudioPlayerIsFormatSupported(playerPtr, format, (IntPtr) (&closestMatchPtr));
		//		if (ret == 0) {
		//			if (closestMatchPtr != IntPtr.Zero) {
		//				WaveFormat result = (WaveFormat) Marshal.PtrToStructure(closestMatchPtr, typeof(WaveFormat));
		//				if (result.WaveFormatTag == AudioEncoding.Extensible) {
		//					result = (WaveFormatExtensible) Marshal.PtrToStructure(closestMatchPtr, typeof(WaveFormatExtensible));
		//				}
		//				Marshal.FreeCoTaskMem(closestMatchPtr);
		//				return result;
		//			} else {
		//				return format;
		//			}
		//		} else {
		//			return null;
		//		}
		//	} else {
		//		if (AudioPlayerIsFormatSupported(playerPtr, format) == 0) {
		//			return format;
		//		}
		//	}

		//	return null;
		//}

		//public WaveFormat GetSupportedFormat(WaveFormat format) {
		//	var result = IsSupportedFormat(format);
		//	if (result != null) return result;

		//	if (ShareMode == ShareMode.Shared) {
		//		AudioPlayerGetMixFormat(playerPtr, out result);
		//		if (result != null) return result;
		//	}

		//	foreach (var temp in GetSupportedFormats()) {
		//		if (temp != null) {
		//			return temp;
		//		}
		//	}

		//	return null;
		//}

		private bool ClientInitialize(WaveFormat inFormat) {
			long requestDuration = durationMillisecond * 10000;
			int result;
			if (ShareMode == ShareMode.Exclusive) {
				result = audioClient.Initialize(ShareMode, DefaultStreamFlags, requestDuration, requestDuration, inFormat);
			} else {
				result = audioClient.Initialize(ShareMode, DefaultStreamFlags, requestDuration, 0, inFormat);
			}
			return result == 0;
		}

		private WaveFormat Initialize(int ratePointer, int bitPointer, int channelPointer) {
			int sampleRate = sampleRatesTemplate[ratePointer];
			int bits = bitsTemplate[bitPointer];
			int channels = channelsTemplate[channelPointer];

			WaveFormat temp;

			if (bits == 24) {
				temp = new WaveFormatExtensible(sampleRate, 32, channels, AudioSubTypes.Pcm, 24);
				//result = IsSupportedFormat(temp);
				//if (result != null) return result;
				if (ClientInitialize(temp)) return temp;
			}

			temp = new WaveFormatExtensible(sampleRate, bits, channels);
			//result = IsSupportedFormat(temp);
			//if (result != null) return result;
			if (ClientInitialize(temp)) return temp;

			return null;
		}

		private WaveFormat Initialize(WaveFormat inFormat) {
			WaveFormat result = null;

			if (supportedFormats.Contains(new FormatKey { Format = inFormat, ShareMode = ShareMode })) {
				if (ClientInitialize(inFormat)) return inFormat;
			}

			int rateIndex = sampleRatesTemplate.IndexOf(inFormat.SampleRate);
			int bitIndex = bitsTemplate.IndexOf(inFormat.ValidBitsPerSample);
			int channelIndex = channelsTemplate.IndexOf(inFormat.Channels);

			for (int channelPointer = channelIndex; channelPointer < channelsTemplate.Count; channelPointer++) {
				for (int ratePointer = rateIndex; ratePointer < sampleRatesTemplate.Count; ratePointer++) {
					for (int bitPointer = bitIndex; bitPointer < bitsTemplate.Count; bitPointer++) {
						result = Initialize(ratePointer, bitPointer, channelPointer);
						if (result != null) goto Success;
					}
				}
			}

			for (int channelPointer = channelIndex; channelPointer >= 0; channelPointer--) {
				for (int ratePointer = rateIndex; ratePointer >= 0; ratePointer--) {
					int bitPointer =
						ratePointer == rateIndex && channelPointer == channelIndex ? bitIndex + 1 : bitIndex;
					for (; bitPointer >= 0; bitPointer--) {
						result = Initialize(ratePointer, bitPointer, channelPointer);
						if (result != null) goto Success;
					}
				}
			}

			return null;
			Success:
			supportedFormats.Add(new FormatKey { Format = result, ShareMode = ShareMode });
			return result;
		}
	}
}
