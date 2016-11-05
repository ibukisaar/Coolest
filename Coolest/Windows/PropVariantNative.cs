using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	static class PropVariantNative {
		[DllImport("ole32.dll")]
		internal static extern int PropVariantClear(ref PropertyVariant pvar);

		[DllImport("ole32.dll")]
		internal static extern int PropVariantClear(IntPtr pvar);
	}
}
