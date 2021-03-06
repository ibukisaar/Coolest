﻿namespace Coolest.Windows {
	/// <summary>
	/// Specifies how to open a property store.
	/// </summary>
	enum StorageAccess {
		/// <summary>
		/// Readable only.
		/// </summary>
		Read = 0x00000000,

		/// <summary>
		/// Writeable but not readable.
		/// </summary>
		Write = 0x00000001,

		/// <summary>
		/// Read- and writeable.
		/// </summary>
		ReadWrite = 0x00000002
	}
}