using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace Coolest.SoundOut.Windows.ComInterface {
	[ComImport]
	[Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[SuppressUnmanagedCodeSecurity]
	interface IAudioRenderClient : IUnknown {
		[SuppressUnmanagedCodeSecurity]
		int GetBuffer(int framesRequested, out IntPtr buffer);
		[SuppressUnmanagedCodeSecurity]
		int ReleaseBuffer(int framesWritten, int flags);
	}
}
