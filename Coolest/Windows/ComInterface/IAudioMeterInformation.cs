﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows.ComInterface {
	[Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioMeterInformation {
		int GetPeakValue(out float pfPeak);
		int GetMeteringChannelCount(out int pnChannelCount);
		int GetChannelsPeakValues(int u32ChannelCount, IntPtr afPeakValues);
		int QueryHardwareSupport(out int pdwHardwareSupportMask);
	}
}