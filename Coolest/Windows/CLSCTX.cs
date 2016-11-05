using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	// ReSharper disable InconsistentNaming
	//copied from http://msdn.microsoft.com/en-us/library/windows/desktop/ms693716%28v=vs.85%29.aspx
	/// <summary>
	/// Values that are used in activation calls to indicate the execution contexts in which an object is to be run.
	/// </summary>
	[Flags]
	public enum ClsCtx : int {
		/// <summary>
		/// The code that creates and manages objects of this class is a DLL that runs in the same process as the caller of the function specifying the class context.
		/// </summary>
		INPROC_SERVER = 0x1,
		/// <summary>
		/// Indicates a handler dll, which runs on the same process as the caller.
		/// </summary>
		INPROC_HANDLER = 0x2,
		/// <summary>
		/// Indicates a server executable, which runs on the same machine but on a different process than the caller. 
		/// </summary>
		LOCAL_SERVER = 0x4,
		/// <summary>
		/// Obsolete.
		/// </summary>
		INPROC_SERVER16 = 0x8,
		/// <summary>
		/// Indicates a server executable, which runs on a different machine than the caller.
		/// </summary>
		REMOTE_SERVER = 0x10,
		/// <summary>
		/// Obsolete.
		/// </summary>
		INPROC_HANDLER16 = 0x20,
		/// <summary>
		/// Reserved.
		/// </summary>
		// RESERVED1 = 0x40,
		/// <summary>
		/// Reserved.
		/// </summary>
		// RESERVED2 = 0x80,
		/// <summary>
		/// Reserved.
		/// </summary>
		// RESERVED3 = 0x100,
		/// <summary>
		/// Reserved.
		/// </summary>
		// RESERVED4 = 0x200,
		/// <summary>
		/// Indicates that code should not be allowed to be downloaded from the Directory Service (if any) or the Internet.
		/// </summary>
		NO_CODE_DOWNLOAD = 0x400,
		/// <summary>
		/// Reserved.
		/// </summary>
		// RESERVED5 = 0x800,
		/// <summary>
		/// Specify if you want the activation to fail if it uses custom marshalling.
		/// </summary>
		NO_CUSTOM_MARSHAL = 0x1000,
		/// <summary>
		/// Enables the downloading of code from the directory service or the Internet.
		/// </summary>
		ENABLE_CODE_DOWNLOAD = 0x2000,
		/// <summary>
		/// Indicates that no log messages about activation failure should be written to the Event Log.
		/// </summary>
		NO_FAILURE_LOG = 0x4000,
		/// <summary>
		/// Indicates that activate-as-activator capability is disabled for this activation only.
		/// </summary>
		DISABLE_AAA = 0x8000,
		/// <summary>
		/// Indicates that activate-as-activator capability is enabled for this activation only.
		/// </summary>
		ENABLE_AAA = 0x10000,
		/// <summary>
		/// Indicates that activation should begin from the default context of the current apartment.
		/// </summary>
		FROM_DEFAULT_CONTEXT = 0x20000,
		/// <summary>
		/// Activate or connect to a 32-bit version of the server; fail if one is not registered.
		/// </summary>
		ACTIVATE_32_BIT_SERVER = 0x40000,
		/// <summary>
		/// Activate or connect to a 64 bit version of the server; fail if one is not registered. 
		/// </summary>
		ACTIVATE_64_BIT_SERVER = 0x80000,
		/// <summary>
		/// When this flag is specified, COM uses the impersonation token of the thread, if one is present, for the activation request made by the thread. When this flag is not specified or if the thread does not have an impersonation token, COM uses the process token of the thread's process for the activation request made by the thread. 
		/// </summary>
		ENABLE_CLOAKING = 0x100000,
		/// <summary>
		/// Indicates activation is for an app container. Reserved for internal use.
		/// </summary>
		APPCONTAINER = 0x400000,
		/// <summary>
		/// Specify this flag for Interactive User activation behavior for As-Activator servers.
		/// </summary>
		ACTIVATE_AAA_AS_IU = 0x800000,
		/// <summary>
		/// Used for loading Proxy/Stub DLLs.
		/// </summary>
		PS_DLL = unchecked((int) 0x80000000),

		/// <summary>
		/// Bitwise combination of the <see cref="INPROC_SERVER"/> and the <see cref="INPROC_HANDLER"/> constants.
		/// </summary>
		INPROC = INPROC_SERVER | INPROC_HANDLER,
		/// <summary>
		/// Bitwise combination of the <see cref="INPROC_SERVER"/>, the <see cref="LOCAL_SERVER"/> and the <see cref="REMOTE_SERVER"/> constants.
		/// </summary>
		SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
		/// <summary>
		/// Bitwise combination of the <see cref="INPROC"/> and the <see cref="SERVER"/> constants.
		/// </summary>
		ALL = INPROC | SERVER
	}
}
