using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

using Coolest.Windows.ComInterface;
using Coolest.Windows;
using Coolest.SoundOut;

namespace Coolest.Windows {
	unsafe public sealed class WasapiOut : AudioPlayerBase {
		private Wasapi wasapi;
		private AudioRenderClient renderClient;
		private AutoResetEvent eventObject = new AutoResetEvent(false);
		private int bufferFrames;

		public ShareMode ShareMode => wasapi.ShareMode;
		public Role Role => wasapi.Device.Role;

		public override WaveFormat OutFormat => wasapi.Format;

		/// <summary>
		/// WASAPI音频输出
		/// </summary>
		/// <param name="shareMode">指定共享模式或独占模式</param>
		/// <param name="eventSync">是否使用事件同步</param>
		/// <param name="role">指定应用场景</param>
		/// <param name="durationMillisecond">缓冲区周期时间</param>
		public WasapiOut(ShareMode shareMode, bool eventSync = true, Role role = Role.Multimedia, int durationMillisecond = 100) {
			wasapi = new Wasapi(Wasapi.GetDefaultDevice(DataFlow.Render, role), StreamFlags.None, shareMode, eventSync, durationMillisecond);
		}

		/// <summary>
		/// WASAPI音频输出，默认使用请求周期100ms，共享模式，音乐场景
		/// </summary>
		/// <param name="waveStream">指定音频源</param>
		public WasapiOut()
			: this(ShareMode.Shared) {
		}

		public void Initialize(IWaveStream waveStream) {
			this.waveStream = waveStream;
			var format = WaveFormatExtensible.Make(waveStream.Format);
			wasapi.Initialize(waveStream.Format);
			if (!format.Equals(OutFormat)) {
				var e = new ResampleEventArgs(waveStream, OutFormat);
				NotifyResample(e);
				this.waveStream = e.Source;
			}

			renderClient = wasapi.AudioClient.AudioRenderClient;
			if (wasapi.EventSync) {
				wasapi.AudioClient.SetEventHandle(eventObject.SafeWaitHandle.DangerousGetHandle());
			}
			bufferFrames = wasapi.AudioClient.BufferSize;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (disposing) {
				eventObject?.WaitOne();
				eventObject?.Close();
				eventObject = null;

				wasapi.Dispose();
				renderClient.Dispose();
			}
		}

		protected override void OnStart() {
			wasapi.AudioClient.Start();
		}

		protected override void OnStop() {
			Thread.Sleep(wasapi.DurationMillisecond / 2);

			wasapi.AudioClient.Stop();
			wasapi.AudioClient.Reset();
		}

		protected override int RequestBufferLength(bool first) {
			if (!first) {
				if (wasapi.EventSync) {
					if (!eventObject.WaitOne(wasapi.DurationMillisecond * 2)) return 0;
				} else {
					Thread.Sleep(wasapi.DurationMillisecond / 2);
				}
			}

			if (ShareMode == ShareMode.Shared && !first) {
				int padding = wasapi.AudioClient.CurrentPadding;
				return (bufferFrames - padding) * OutFormat.BlockAlign;
			} else {
				return bufferFrames * OutFormat.BlockAlign;
			}
		}

		protected override void WriteBuffer(byte[] buffer, int writeLength) {
			int framesCount = writeLength / OutFormat.BlockAlign;
			if (framesCount > 0) {
				IntPtr bufferPtr = renderClient.GetBuffer(framesCount);
				Marshal.Copy(buffer, 0, bufferPtr, writeLength);
				renderClient.ReleaseBuffer(framesCount, 0);
			}
		}

	}
}
