using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut.Windows.ComInterface {
	[ComImport]
	[Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[SuppressUnmanagedCodeSecurity]
	unsafe interface IAudioClient : IUnknown {
		int Initialize(ShareMode shareMode, AudioClientStreamFlags streamFlags, long hnsBufferDuration, long hnsPeriodicity, WaveFormat format, void* audioSessionGuid = null);
		int GetBufferSize([Out] out int bufferFrames);
		int GetStreamLatency([Out] out long phnsLatency);
		int GetCurrentPadding([Out] out int paddingFrames);
		int IsFormatSupported(ShareMode shareMode, WaveFormat format, IntPtr closedMatch);
		int GetMixFormat([Out] out WaveFormat deviceFormat);
		int GetDevicePeriod([Out] out long phnsDefaultDevicePeriod, [Out] out long phnsMinimumDevicePeriod);
		int Start();
		int Stop();
		int Reset();
		int SetEventHandle(IntPtr eventHandle);
		int GetService(ref Guid guid, [Out] out IntPtr ppv);
	}
}
