﻿using System.Runtime.InteropServices;

namespace Coolest.Windows.ComInterface {
	/// <summary>
	/// Windows CoreAudio IAudioSessionNotification interface
	/// Defined in AudioPolicy.h
	/// </summary>
	[Guid("641DD20B-4D41-49CC-ABA3-174B9477BB08"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IAudioSessionNotification {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newSession">session being added</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnSessionCreated(IAudioSessionControl newSession);
	}
}