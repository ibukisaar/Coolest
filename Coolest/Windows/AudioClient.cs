using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	class AudioClient : DisposableObject {
		private IAudioClient audioClientInterface;
		private AudioStreamVolume audioStreamVolume;
		private AudioClockClient audioClockClient;
		private AudioRenderClient audioRenderClient;
		private AudioCaptureClient audioCaptureClient;
		private WaveFormat mixFormat;
		private ShareMode shareMode;


		internal AudioClient(IAudioClient audioClientInterface) {
			this.audioClientInterface = audioClientInterface;
		}

		/// <summary>
		/// Retrieves the stream format that the audio engine uses for its internal processing of shared-mode streams.
		/// Can be called before initialize
		/// </summary>
		public WaveFormat MixFormat {
			get {
				if (mixFormat == null) {
					audioClientInterface.GetMixFormat(out mixFormat);
				}
				return mixFormat;
			}
		}

		/// <summary>
		/// Initializes the Audio Client
		/// </summary>
		/// <param name="shareMode">Share Mode</param>
		/// <param name="streamFlags">Stream Flags</param>
		/// <param name="bufferDuration">Buffer Duration</param>
		/// <param name="periodicity">Periodicity</param>
		/// <param name="waveFormat">Wave Format</param>
		/// <param name="audioSessionGuid">Audio Session GUID (can be null)</param>
		public int Initialize(ShareMode shareMode,
			StreamFlags streamFlags,
			long bufferDuration,
			long periodicity,
			WaveFormat waveFormat,
			Guid audioSessionGuid) {

			this.shareMode = shareMode;
			int hret = audioClientInterface.Initialize(shareMode, streamFlags, bufferDuration, periodicity, waveFormat, ref audioSessionGuid);
			mixFormat = null;
			return hret;
		}

		/// <summary>
		/// Retrieves the size (maximum capacity) of the audio buffer associated with the endpoint. (must initialize first)
		/// </summary>
		public int BufferSize {
			get {
				int bufferSize;
				Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out bufferSize));
				return bufferSize;
			}
		}

		/// <summary>
		/// Retrieves the maximum latency for the current stream and can be called any time after the stream has been initialized.
		/// </summary>
		public long StreamLatency {
			get {
				long streamLatency;
				audioClientInterface.GetStreamLatency(out streamLatency);
				return streamLatency;
			}
		}

		/// <summary>
		/// Retrieves the number of frames of padding in the endpoint buffer (must initialize first)
		/// </summary>
		public int CurrentPadding {
			get {
				int currentPadding;
				Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out currentPadding));
				return currentPadding;
			}
		}

		/// <summary>
		/// Retrieves the length of the periodic interval separating successive processing passes by the audio engine on the data in the endpoint buffer.
		/// (can be called before initialize)
		/// </summary>
		public long DefaultDevicePeriod {
			get {
				long defaultDevicePeriod;
				long minimumDevicePeriod;
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
				return defaultDevicePeriod;
			}
		}

		/// <summary>
		/// Gets the minimum device period 
		/// (can be called before initialize)
		/// </summary>
		public long MinimumDevicePeriod {
			get {
				long defaultDevicePeriod;
				long minimumDevicePeriod;
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
				return minimumDevicePeriod;
			}
		}

		/// <summary>
		/// Returns the AudioStreamVolume service for this AudioClient.
		/// </summary>
		/// <remarks>
		/// This returns the AudioStreamVolume object ONLY for shared audio streams.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when an exclusive audio stream is being used.
		/// </exception>
		public AudioStreamVolume AudioStreamVolume {
			get {
				if (shareMode == ShareMode.Exclusive) {
					throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
				}
				if (audioStreamVolume == null) {
					object audioStreamVolumeInterface;
					int hret = audioClientInterface.GetService(typeof(IAudioStreamVolume).GUID, out audioStreamVolumeInterface);
					Marshal.ThrowExceptionForHR(hret);
					audioStreamVolume = new AudioStreamVolume((IAudioStreamVolume) audioStreamVolumeInterface);
				}
				return audioStreamVolume;
			}
		}

		/// <summary>
		/// Gets the AudioClockClient service
		/// </summary>
		public AudioClockClient AudioClockClient {
			get {
				if (audioClockClient == null) {
					object audioClockClientInterface;
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(typeof(IAudioClock).GUID, out audioClockClientInterface));
					audioClockClient = new AudioClockClient((IAudioClock) audioClockClientInterface);
				}
				return audioClockClient;
			}
		}

		/// <summary>
		/// Gets the AudioRenderClient service
		/// </summary>
		public AudioRenderClient AudioRenderClient {
			get {
				if (audioRenderClient == null) {
					object audioRenderClientInterface;
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(typeof(IAudioRenderClient).GUID, out audioRenderClientInterface));
					audioRenderClient = new AudioRenderClient((IAudioRenderClient) audioRenderClientInterface);
				}
				return audioRenderClient;
			}
		}

		/// <summary>
		/// Gets the AudioCaptureClient service
		/// </summary>
		public AudioCaptureClient AudioCaptureClient {
			get {
				if (audioCaptureClient == null) {
					object audioCaptureClientInterface;
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(typeof(IAudioCaptureClient).GUID, out audioCaptureClientInterface));
					audioCaptureClient = new AudioCaptureClient((IAudioCaptureClient) audioCaptureClientInterface);
				}
				return audioCaptureClient;
			}
		}

		/// <summary>
		/// Determines whether if the specified output format is supported
		/// </summary>
		/// <param name="shareMode">The share mode.</param>
		/// <param name="desiredFormat">The desired format.</param>
		/// <returns>True if the format is supported</returns>
		public bool IsFormatSupported(ShareMode shareMode, WaveFormat desiredFormat) {
			WaveFormatExtensible closestMatchFormat;
			return IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
		}

		unsafe public bool IsFormatSupported(ShareMode shareMode, WaveFormat desiredFormat, out WaveFormatExtensible closestMatchFormat) {
			int hret;
			IntPtr closestMatchPtr = IntPtr.Zero;

			if (shareMode == ShareMode.Exclusive) {
				hret = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, IntPtr.Zero);
			} else {
				hret = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, (IntPtr) (&closestMatchPtr));
			}

			if (closestMatchPtr != IntPtr.Zero) {
				closestMatchFormat = Marshal.PtrToStructure<WaveFormatExtensible>(closestMatchPtr);
				Marshal.FreeCoTaskMem(closestMatchPtr);
			} else {
				closestMatchFormat = null;
			}

			if (hret == 0 || closestMatchPtr != null) {
				return true;
			} else if (hret == 1 || hret == (int) AudioClientErrors.UnsupportedFormat) {
				return false;
			}
			Marshal.ThrowExceptionForHR(hret);
			return false;
		}

		/// <summary>
		/// Starts the audio stream
		/// </summary>
		public void Start() {
			audioClientInterface.Start();
		}

		/// <summary>
		/// Stops the audio stream.
		/// </summary>
		public void Stop() {
			audioClientInterface.Stop();
		}

		/// <summary>
		/// Set the Event Handle for buffer synchro.
		/// </summary>
		/// <param name="eventWaitHandle">The Wait Handle to setup</param>
		public void SetEventHandle(IntPtr eventWaitHandle) {
			audioClientInterface.SetEventHandle(eventWaitHandle);
		}

		/// <summary>
		/// Resets the audio stream
		/// Reset is a control method that the client calls to reset a stopped audio stream. 
		/// Resetting the stream flushes all pending data and resets the audio clock stream 
		/// position to 0. This method fails if it is called on a stream that is not stopped
		/// </summary>
		public void Reset() {
			audioClientInterface.Reset();
		}

		protected override void Dispose(bool disposing) {
			if (audioClientInterface != null) {
				if (disposing) {
					audioClockClient?.Dispose();
					audioRenderClient?.Dispose();
					audioCaptureClient?.Dispose();
					audioStreamVolume?.Dispose();
				}

				ReleaseComObject(ref audioClientInterface);
			}
		}
	}
}
