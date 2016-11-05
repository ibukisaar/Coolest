using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows.ComInterface {

	[Guid("93014887-242D-4068-8A15-CF5E93B90FE3")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IAudioStreamVolume {
		[PreserveSig]
		int GetChannelCount(out uint dwCount);

		[PreserveSig]
		int SetChannelVolume(uint dwIndex, float fLevel);

		[PreserveSig]
		int GetChannelVolume(uint dwIndex, out float fLevel);

		[PreserveSig]
		int SetAllVoumes(uint dwCount, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4, SizeParamIndex = 0)] float[] fVolumes);

		[PreserveSig]
		int GetAllVolumes(uint dwCount, [MarshalAs(UnmanagedType.LPArray)] float[] pfVolumes);
	}
}
