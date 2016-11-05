using System;
using System.Runtime.InteropServices;

namespace Coolest {
	public abstract class DisposableObject : IDisposable {
		~DisposableObject() {
			Dispose(false);
		}

		protected abstract void Dispose(bool disposing);

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public static void ReleaseComObject(object comInterface) {
			Marshal.ReleaseComObject(comInterface);
		}

		public static void ReleaseComObject<T>(ref T comInterface) where T : class {
			if (comInterface != null) {
				ReleaseComObject(comInterface);
				comInterface = null;
			}
		}
	}
}
