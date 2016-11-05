using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coolest.SoundIn {
	public abstract class AudioCaptureBase : DisposableObject {
		protected object locker = new object();
		protected Thread captureThread;
		protected bool capture = false;
		protected byte[] cache = new byte[0];
		protected DataAvailableEventArgs eventArgs = new DataAvailableEventArgs();

		public event EventHandler<DataAvailableEventArgs> DataAvailable;

		public event EventHandler<CaptureStoppedEventArgs> Stopped;

		public abstract WaveFormat Format { get; }

		protected override void Dispose(bool disposing) {
			if (capture) {
				Stop();
			}
		}

		private void CaptureProcess() {
			try {
				while (capture) {
					int length = RequestBufferLength();
					if (length > 0) {
						if (cache.Length < length)
							cache = new byte[length];
						Read(cache, length);

						eventArgs.Data = cache;
						eventArgs.Bytes = length;
						DataAvailable?.Invoke(this, eventArgs);
					}
				}
			} catch (Exception e) {
				Stopped?.Invoke(this, new CaptureStoppedEventArgs(e));
			}
		}

		public void Start() {
			lock (locker) {
				if (capture) return;
				capture = true;

				OnStart();

				captureThread = new Thread(CaptureProcess);
				captureThread.Priority = ThreadPriority.Highest;
				captureThread.Start();
			}
		}

		public void Stop() {
			lock (locker) {
				if (!capture) return;
				capture = false;

				captureThread.Join();

				OnStop();
				Stopped?.Invoke(this, CaptureStoppedEventArgs.Empty);
			}
		}

		protected abstract void OnStart();

		protected abstract void OnStop();

		protected abstract int RequestBufferLength();

		protected abstract void Read(byte[] buffer, int bytes);
	}
}
