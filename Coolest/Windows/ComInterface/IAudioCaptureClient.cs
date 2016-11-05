using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace Coolest.Windows.ComInterface {
	[ComImport]
	[Guid("C8ADBD64-E71E-48a0-A4DE-185C395CD317")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	unsafe interface IAudioCaptureClient {
		[SuppressUnmanagedCodeSecurity]
		int GetBuffer(out IntPtr buffer, out int numFramesToRead, out AudioClientBufferFlags flags, out long pu64DevicePosition, out long pu64QPCPosition);
		[SuppressUnmanagedCodeSecurity]
		int ReleaseBuffer(int numFramesRead);
		[SuppressUnmanagedCodeSecurity]
		int GetNextPacketSize(out int numFramesInNextPacket);
	}
}
