﻿using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Render Client
	/// </summary>
	class AudioRenderClient : DisposableObject {
		IAudioRenderClient audioRenderClientInterface;

		internal AudioRenderClient(IAudioRenderClient audioRenderClientInterface) {
			this.audioRenderClientInterface = audioRenderClientInterface;
		}

		/// <summary>
		/// Gets a pointer to the buffer
		/// </summary>
		/// <param name="numFramesRequested">Number of frames requested</param>
		/// <returns>Pointer to the buffer</returns>
		public IntPtr GetBuffer(int numFramesRequested) {
			IntPtr bufferPointer;
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.GetBuffer(numFramesRequested, out bufferPointer));
			return bufferPointer;
		}

		/// <summary>
		/// Release buffer
		/// </summary>
		/// <param name="numFramesWritten">Number of frames written</param>
		/// <param name="bufferFlags">Buffer flags</param>
		public void ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags) {
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.ReleaseBuffer(numFramesWritten, bufferFlags));
		}

		protected override void Dispose(bool disposing) {
			ReleaseComObject(ref audioRenderClientInterface);
		}
	}
}
