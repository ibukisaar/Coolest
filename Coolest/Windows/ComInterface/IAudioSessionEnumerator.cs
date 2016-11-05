using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows.ComInterface {
	[Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionEnumerator {
		int GetCount(out int sessionCount);

		int GetSession(int sessionCount, out IAudioSessionControl session);
	}
}
