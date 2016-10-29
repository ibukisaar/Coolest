using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut.Windows {
	/// <summary>
	///     Enables a client to create and initialize an audio stream between an audio application and the audio engine (for a
	///     shared-mode stream) or the hardware buffer of an audio endpoint device (for an exclusive-mode stream). For more
	///     information, see
	///     <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd370865(v=vs.85).aspx" />.
	/// </summary>
	[ComImport]
	[Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAudioClient : IUnknown {
		int GetMixFormat(out IntPtr format);
	}
}
