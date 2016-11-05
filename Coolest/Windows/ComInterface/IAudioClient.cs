
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows.ComInterface {
	[ComImport]
	[Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[SuppressUnmanagedCodeSecurity]
	unsafe interface IAudioClient {
		[SuppressUnmanagedCodeSecurity]
		[PreserveSig]
		int Initialize(ShareMode shareMode, StreamFlags streamFlags, long hnsBufferDuration, long hnsPeriodicity, WaveFormat format, [In] ref Guid audioSessionGuid);
		[SuppressUnmanagedCodeSecurity]
		int GetBufferSize(out int bufferFrames);
		[SuppressUnmanagedCodeSecurity]
		int GetStreamLatency(out long phnsLatency);
		[SuppressUnmanagedCodeSecurity]
		int GetCurrentPadding(out int paddingFrames);
		[SuppressUnmanagedCodeSecurity]
		[PreserveSig]
		int IsFormatSupported(ShareMode shareMode, WaveFormat format, IntPtr closestMatchFormat);
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
		[PreserveSig]
		int GetService([In] ref Guid interfaceId, [Out, MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
	}
}
