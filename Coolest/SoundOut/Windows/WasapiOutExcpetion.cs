using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut.Windows {
	public sealed class WasapiOutExcpetion : Exception {
		const string DllName = "coolest.win32.dll";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GetErrorString(uint errorCode, out IntPtr messagePtr);

		public uint ErrorCode { get; }

		public WasapiOutExcpetion(uint errorCode) : base($"Error 0x{errorCode:x8}:{GetMessage(errorCode)}") {
			ErrorCode = errorCode;
		}

		private static string GetMessage(uint errorCode) {
			IntPtr messagePtr;
			GetErrorString(errorCode, out messagePtr);
			return Marshal.PtrToStringAnsi(messagePtr);
		}
	}
}
