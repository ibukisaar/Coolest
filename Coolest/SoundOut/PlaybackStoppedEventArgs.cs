using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coolest.SoundOut {
	public class PlaybackStoppedEventArgs : StoppedEventArgs {
		public IWaveStream WaveStream { get; }

		public PlaybackStoppedEventArgs(IWaveStream waveStream, Exception exception = null) : base(exception) {
			WaveStream = waveStream;
		}
	}
}
