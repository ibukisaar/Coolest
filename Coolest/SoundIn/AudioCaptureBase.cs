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
		protected DataAvailableEventArgs eventArgs = new DataAvailableEventArgs();

		public event EventHandler<DataAvailableEventArgs> DataAvailable;

		public event EventHandler<CaptureStoppedEventArgs> Stopped;

		public delegate void ReadHandler(byte[] buffer, int length);

		public abstract WaveFormat Format { get; }

		protected override void Dispose(bool disposing) {
			if (capture) {
				Stop();
			}
		}

		private void CaptureProcess() {
			try {
				while (capture) {
					Read((data, length) => {
						if (DataAvailable != null) {
							eventArgs.Data = data;
							eventArgs.Bytes = length;
							DataAvailable(this, eventArgs);
						}
					});
				}
			} catch (Exception e) {
				Stopped?.Invoke(this, new CaptureStoppedEventArgs(e));
				throw e;
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

		protected abstract void Read(ReadHandler readHandler);
	}
}
