using Coolest.Windows.ComInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	/// <summary>
	/// Audio Capture Client
	/// </summary>
	class AudioCaptureClient : DisposableObject {
		IAudioCaptureClient audioCaptureClientInterface;

		internal AudioCaptureClient(IAudioCaptureClient audioCaptureClientInterface) {
			this.audioCaptureClientInterface = audioCaptureClientInterface;
		}

		/// <summary>
		/// Gets a pointer to the buffer
		/// </summary>
		/// <returns>Pointer to the buffer</returns>
		public IntPtr GetBuffer(
			out int numFramesToRead,
			out AudioClientBufferFlags bufferFlags,
			out long devicePosition,
			out long qpcPosition) {
			IntPtr bufferPointer;
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out bufferPointer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
			return bufferPointer;
		}

		/// <summary>
		/// Gets a pointer to the buffer
		/// </summary>
		/// <param name="numFramesToRead">Number of frames to read</param>
		/// <param name="bufferFlags">Buffer flags</param>
		/// <returns>Pointer to the buffer</returns>
		public IntPtr GetBuffer(
			out int numFramesToRead,
			out AudioClientBufferFlags bufferFlags) {
			IntPtr bufferPointer;
			long devicePosition;
			long qpcPosition;
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out bufferPointer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
			return bufferPointer;
		}

		/// <summary>
		/// Gets the size of the next packet
		/// </summary>
		public int GetNextPacketSize() {
			int numFramesInNextPacket;
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetNextPacketSize(out numFramesInNextPacket));
			return numFramesInNextPacket;
		}

		/// <summary>
		/// Release buffer
		/// </summary>
		/// <param name="numFramesWritten">Number of frames written</param>
		public void ReleaseBuffer(int numFramesWritten) {
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.ReleaseBuffer(numFramesWritten));
		}

		protected override void Dispose(bool disposing) {
			ReleaseComObject(ref audioCaptureClientInterface);
		}
	}
}
