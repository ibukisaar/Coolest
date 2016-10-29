using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coolest.SoundOut {
	public static class SupportedFormatSearcher {
		private readonly static Dictionary<WaveFormat, WaveFormat> supportedFormats = new Dictionary<WaveFormat, WaveFormat>();

		public static WaveFormat Supported(AudioPlayerBase player, WaveFormat format) {
			return null;
		}
	}
}
