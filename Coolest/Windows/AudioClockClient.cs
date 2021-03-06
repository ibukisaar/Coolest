﻿using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Clock Client
	/// </summary>
	class AudioClockClient : DisposableObject {
		IAudioClock audioClockClientInterface;

		internal AudioClockClient(IAudioClock audioClockClientInterface) {
			this.audioClockClientInterface = audioClockClientInterface;
		}

		/// <summary>
		/// Characteristics
		/// </summary>
		public int Characteristics {
			get {
				uint characteristics;
				Marshal.ThrowExceptionForHR(audioClockClientInterface.GetCharacteristics(out characteristics));
				return (int) characteristics;
			}
		}

		/// <summary>
		/// Frequency
		/// </summary>
		public ulong Frequency {
			get {
				ulong freq;
				Marshal.ThrowExceptionForHR(audioClockClientInterface.GetFrequency(out freq));
				return freq;
			}
		}

		/// <summary>
		/// Get Position
		/// </summary>
		public bool GetPosition(out ulong position, out ulong qpcPosition) {
			var hr = audioClockClientInterface.GetPosition(out position, out qpcPosition);
			if (hr == -1) return false;
			Marshal.ThrowExceptionForHR(hr);
			return true;
		}

		/// <summary>
		/// Adjusted Position
		/// </summary>
		public ulong AdjustedPosition {
			get {
				// figure out ticks per byte (for later)
				var byteLatency = (TimeSpan.TicksPerSecond / Frequency);

				ulong pos, qpos;
				int cnt = 0;
				while (!GetPosition(out pos, out qpos)) {
					if (++cnt == 5) {
						// we've tried too many times, so now we have to just run with what we have...
						break;
					}
				}

				if (Stopwatch.IsHighResolution) {
					// cool, we can adjust our position appropriately

					// get the current qpc count (in ticks)
					var qposNow = (ulong) ((Stopwatch.GetTimestamp() * 10000000M) / Stopwatch.Frequency);

					// find out how many ticks has passed since the device reported the position
					var qposDiff = (qposNow - qpos) / 100;

					// find out how many byte would have played in that time span
					var bytes = qposDiff / byteLatency;

					// add it to the position
					pos += bytes;
				}
				return pos;
			}
		}

		/// <summary>
		/// Can Adjust Position
		/// </summary>
		public bool CanAdjustPosition {
			get { return Stopwatch.IsHighResolution; }
		}

		protected override void Dispose(bool disposing) {
			if (audioClockClientInterface != null) {
				ReleaseComObject(audioClockClientInterface);
				audioClockClientInterface = null;
			}
		}
	}
}
