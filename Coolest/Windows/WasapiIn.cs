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
		private Wasapi wasapi;
		private AudioCaptureClient captureClient;
		private AutoResetEvent eventObject = new AutoResetEvent(false);
		private byte[] recordBuffer;

		public override WaveFormat Format => wasapi.Format;

		public WasapiIn(bool loopback = true, ShareMode shareMode = ShareMode.Shared, bool eventSync = false, Role role = Role.Multimedia, int durationMillisecond = 30) {
			wasapi = new Wasapi(
				Wasapi.GetDefaultDevice(loopback ? DataFlow.Render : DataFlow.Capture, role),
				loopback ? StreamFlags.StreamFlagsLoopback : StreamFlags.None,
				shareMode, eventSync, durationMillisecond);
		}

		public void Initialize() {
			var format = WaveFormatExtensible.Make(wasapi.AudioClient.MixFormat);
			wasapi.Initialize(format);

			captureClient = wasapi.AudioClient.AudioCaptureClient;
			if (wasapi.EventSync) {
				wasapi.AudioClient.SetEventHandle(eventObject.SafeWaitHandle.DangerousGetHandle());
			}

			recordBuffer = new byte[wasapi.AudioClient.BufferSize * wasapi.Format.BlockAlign];
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				eventObject?.WaitOne();
				eventObject?.Close();
				eventObject = null;

				wasapi.Dispose();
				captureClient.Dispose();
			}

			base.Dispose(disposing);
		}

		private static IAudioCaptureClient CreateRenderClient(IAudioClient audioClient) {
			object client;
			audioClient.GetService(typeof(IAudioCaptureClient).GUID, out client);
			return client as IAudioCaptureClient;
		}

		protected override void OnStart() {
			wasapi.AudioClient.Start();
		}

		protected override void OnStop() {
			wasapi.AudioClient.Stop();
		}

		protected override void Read(ReadHandler readHandler) {
			int frameCount = wasapi.AudioClient.BufferSize;
			int durationMs = 1000 * frameCount / Format.SampleRate;

			if (wasapi.EventSync) {
				if (!eventObject.WaitOne(durationMs * 2, false)) {
					return;
				}
			} else {
				Thread.Sleep(durationMs / 2);
			}

			int offset = 0;

			while (true) {
				int packetSize = captureClient.GetNextPacketSize();
				if (packetSize == 0) break;

				int frames;
				AudioClientBufferFlags flags;
				IntPtr bufferPtr = captureClient.GetBuffer(out frames, out flags);

				int readBytes = frames * Format.BlockAlign;
				if (recordBuffer.Length - offset < readBytes && offset > 0) {
					readHandler(recordBuffer, offset);
					offset = 0;
				}

				if ((flags & AudioClientBufferFlags.Silent) != 0) {
					Array.Clear(recordBuffer, offset, readBytes);
				} else {
					Marshal.Copy(bufferPtr, recordBuffer, offset, readBytes);
				}
				offset += readBytes;
				captureClient.ReleaseBuffer(frames);
			}
			if (offset > 0) {
				readHandler(recordBuffer, offset);
			}
		}
	}
}
