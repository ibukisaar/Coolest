using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest {
	unsafe public class TestStream : IWaveStream {
		const double f = 440;
		int offset = 0;

		public bool CanSeek => false;

		public WaveFormat Format { get; } = new WaveFormat(48000, 24, 2, AudioEncoding.Pcm);

		public int Read(byte[] buffer, int start, int length) {
			int framesCount = length / 8;
			var channels = Format.Channels;
			var dataLength = framesCount * channels;

			fixed (byte* p = &buffer[start]) {
				for (int i = 0; i < dataLength; i += channels, offset++) {
					double value = Math.Sin(440.0 / Format.SampleRate * offset * (2 * Math.PI));
					for (int j = 0; j < channels; j++) {
						// ((float*) p)[i + j] = (float) (value * 0.1);
						// ((float*) p)[i + j] = 1;

						// ((short*) p)[i + j] = (short) (short.MaxValue * value);
						((int*) p)[i + j] = (int) (int.MaxValue * value);
						//int temp = (int) (0x7fffff * value);
						//p[(i + j) * 3 + 0] = ((byte*) &temp)[0];
						//p[(i + j) * 3 + 1] = ((byte*) &temp)[1];
						//p[(i + j) * 3 + 2] = ((byte*) &temp)[2];
					}
				}
			}

			return length;
		}

		public void Resample(WaveFormat newFormat) {
			throw new NotImplementedException();
		}
	}
}
