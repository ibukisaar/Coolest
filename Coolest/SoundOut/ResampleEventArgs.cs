using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut {
	public class ResampleEventArgs : EventArgs {
		public WaveFormat OutFormat { get; }
		public IWaveStream Source { get; set; }

		public ResampleEventArgs(IWaveStream Source , WaveFormat OutFormat) {
			this.Source = Source;
			this.OutFormat = OutFormat;
		}
	}
}
