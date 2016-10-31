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
		[SuppressUnmanagedCodeSecurity]
		[PreserveSig]
		int Initialize(ShareMode shareMode, AudioClientStreamFlags streamFlags, long hnsBufferDuration, long hnsPeriodicity, WaveFormat format, void* audioSessionGuid = null);
		[SuppressUnmanagedCodeSecurity]
		int GetBufferSize(out int bufferFrames);
		[SuppressUnmanagedCodeSecurity]
		int GetStreamLatency(out long phnsLatency);
		[SuppressUnmanagedCodeSecurity]
		int GetCurrentPadding(out int paddingFrames);
		[SuppressUnmanagedCodeSecurity]
		int IsFormatSupported(ShareMode shareMode, WaveFormat format, IntPtr closedMatch);
		[SuppressUnmanagedCodeSecurity]
		int GetMixFormat(out WaveFormat deviceFormat);
		[SuppressUnmanagedCodeSecurity]
		int GetDevicePeriod(out long phnsDefaultDevicePeriod, out long phnsMinimumDevicePeriod);
		[SuppressUnmanagedCodeSecurity]
		int Start();
		[SuppressUnmanagedCodeSecurity]
		int Stop();
		[SuppressUnmanagedCodeSecurity]
		int Reset();
		[SuppressUnmanagedCodeSecurity]
		int SetEventHandle(IntPtr eventHandle);
		[SuppressUnmanagedCodeSecurity]
		int GetService(ref Guid guid, out IUnknown ppv);
	}
}
