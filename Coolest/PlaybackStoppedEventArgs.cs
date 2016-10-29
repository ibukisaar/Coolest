using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coolest {
	public class PlaybackStoppedEventArgs : EventArgs {
		public IWaveStream WaveStream { get; }

		public PlaybackStoppedEventArgs(IWaveStream waveStream) {
			this.WaveStream = waveStream;
		}
	}
}
