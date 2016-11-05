using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coolest.SoundIn;
using Coolest.Windows.ComInterface;
using System.Threading;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	unsafe public sealed class WasapiIn : AudioCaptureBase {
		private IAudioClient audioClient;
		private IAudioCaptureClient captureClient;
		private WaveFormat format;
		private AutoResetEvent eventObject = new AutoResetEvent(false);
		private IntPtr buffer;
		private int frames;
		private int durationMillisecond;

		public override WaveFormat Format => format;

		public WasapiIn(bool loopback = true, int durationMillisecond = 100) {
			this.durationMillisecond = durationMillisecond;

			audioClient = CreateAudioClient(loopback);
			int ret = audioClient.GetMixFormat(out format);
			format.ExtraSize = 0;
			format.WaveFormatTag = AudioEncoding.IeeeFloat;

			StreamFlags flags = StreamFlags.None;
			if (loopback) flags |= StreamFlags.StreamFlagsLoopback;
			ret = audioClient.Initialize(ShareMode.Shared, flags, durationMillisecond * 10000, 0, format, Guid.Empty);
			captureClient = CreateRenderClient(audioClient);
			// audioClient.SetEventHandle(eventObject.SafeWaitHandle.DangerousGetHandle());
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				eventObject?.Close();
				eventObject = null;
			}

			base.Dispose(disposing);

			if (audioClient != null) {
				Marshal.FinalReleaseComObject(audioClient);
				audioClient = null;
			}
			if (captureClient != null) {
				Marshal.FinalReleaseComObject(captureClient);
				captureClient = null;
			}
		}

		private static IAudioClient CreateAudioClient(bool loopback) {
			IMMDeviceEnumerator deviceEnumer = null;
			IMMDevice device = null;
			try {
				object client;
				deviceEnumer = (IMMDeviceEnumerator) new MMDeviceEnumerator();
				deviceEnumer.GetDefaultAudioEndpoint(loopback ? DataFlow.Render : DataFlow.Capture, Role.Console, out device);
				device.Activate(typeof(IAudioClient).GUID, ClsCtx.ALL, IntPtr.Zero, out client);
				return client as IAudioClient;
			} finally {
				Marshal.FinalReleaseComObject(deviceEnumer);
				Marshal.FinalReleaseComObject(device);
			}
		}

		private static IAudioCaptureClient CreateRenderClient(IAudioClient audioClient) {
			object client;
			audioClient.GetService(typeof(IAudioCaptureClient).GUID, out client);
			return client as IAudioCaptureClient;
		}

		protected override void OnStart() {
			audioClient.Start();
		}

		protected override void OnStop() {
			audioClient.Stop();
		}

		protected override void Read(byte[] buffer, int bytes) {
			Marshal.Copy(this.buffer, buffer, 0, bytes);
			captureClient.ReleaseBuffer(frames);
		}

		protected override int RequestBufferLength() {
			int packets;
			captureClient.GetNextPacketSize(out packets);
			if (packets == 0)
				Thread.Sleep(durationMillisecond / 2);

			long a, b;
			AudioClientBufferFlags flags;
			captureClient.GetBuffer(out buffer, out frames, out flags, out a, out b);
			if (!flags.HasFlag(AudioClientBufferFlags.Silent)) {
				return frames * format.BlockAlign;
			} else {
				captureClient.ReleaseBuffer(frames);
			}
			return 0;
		}
	}
}
