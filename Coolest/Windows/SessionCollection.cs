using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	class SessionCollection {
		readonly IAudioSessionEnumerator audioSessionEnumerator;

		internal SessionCollection(IAudioSessionEnumerator realEnumerator) {
			audioSessionEnumerator = realEnumerator;
		}

		/// <summary>
		/// Returns session at index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AudioSessionControl this[int index] {
			get {
				IAudioSessionControl result;
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetSession(index, out result));
				return new AudioSessionControl(result);
			}
		}

		/// <summary>
		/// Number of current sessions.
		/// </summary>
		public int Count {
			get {
				int result;
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetCount(out result));
				return result;
			}
		}
	}
}