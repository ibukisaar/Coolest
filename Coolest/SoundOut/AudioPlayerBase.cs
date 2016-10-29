﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Coolest.SoundOut {
	public abstract class AudioPlayerBase : DisposableObject {
		protected object locker = new object();
		protected Thread playThread;
		protected bool playing = false;
		protected IWaveStream waveStream;

		public bool Playing => playing;

		public virtual WaveFormat OutFormat => waveStream.Format;

		public event EventHandler<PlaybackStoppedEventArgs> Stopped;


		public AudioPlayerBase(IWaveStream waveStream) {
			this.waveStream = waveStream;
		}

		protected override void Dispose(bool disposing) {
			playing = false;

			if (disposing) {
				if (!playThread.Join(1000)) playThread.Abort();
			}
		}

		private void PlayProcess() {
			int length = RequestBufferLength(true);
			if (length <= 0) throw new InvalidOperationException("播放线程无法初始化缓冲区。");

			if (!playing) return;

			byte[] buffer = new byte[length];
			int writeLength;
			if (!Write(ref buffer, length, out writeLength)) {
				playing = false;
				return;
			}

			OnPlay();

			while (playing) {
				length = RequestBufferLength(false);
				if (length <= 0) continue;

				if (!Write(ref buffer, length, out writeLength)) {
					playing = false;
					return;
				}
			}
		}

		private bool Write(ref byte[] buffer, int length, out int writeLength) {
			if (buffer.Length < length) {
				buffer = new byte[length];
			}

			bool isSuccess = true;
			int offset = 0;
			while (offset < length) {
				int result = waveStream.Read(buffer, offset, length - offset);
				if (result < 0) {
					if (offset > 0) {
						Array.Clear(buffer, offset, length - offset);
						isSuccess = false;
						break;
					} else {
						writeLength = 0;
						return false;
					}
				}
				offset += result;
			}
			writeLength = offset;
			WriteBuffer(buffer, length);
			return isSuccess;
		}

		public void Play() {
			lock (locker) {
				if (playing) return;
				playing = true;

				playThread = new Thread(PlayProcess);
				playThread.Priority = ThreadPriority.Highest;
				playThread.Start();
			}
		}

		public void Stop() {
			lock (locker) {
				if (!playing) return;
				playing = false;

				OnStop();
				Stopped?.Invoke(this, new PlaybackStoppedEventArgs(waveStream));
			}
		}

		protected abstract void OnPlay();

		protected abstract void OnStop();

		protected abstract int RequestBufferLength(bool first);

		protected abstract void WriteBuffer(byte[] buffer, int writeLength);
	}
}
