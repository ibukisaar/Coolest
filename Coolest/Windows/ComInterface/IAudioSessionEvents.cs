﻿using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows.ComInterface {
	/// <summary>
	/// Defines constants that indicate the current state of an audio session.
	/// </summary>
	/// <remarks>
	/// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370792.aspx
	/// </remarks>
	enum AudioSessionState {
		/// <summary>
		/// The audio session is inactive.
		/// </summary>
		AudioSessionStateInactive = 0,

		/// <summary>
		/// The audio session is active.
		/// </summary>
		AudioSessionStateActive = 1,

		/// <summary>
		/// The audio session has expired.
		/// </summary>
		AudioSessionStateExpired = 2
	}

	/// <summary>
	/// Defines constants that indicate a reason for an audio session being disconnected.
	/// </summary>
	/// <remarks>
	/// MSDN Reference: Unknown
	/// </remarks>
	enum AudioSessionDisconnectReason {
		/// <summary>
		/// The user removed the audio endpoint device.
		/// </summary>
		DisconnectReasonDeviceRemoval = 0,

		/// <summary>
		/// The Windows audio service has stopped.
		/// </summary>
		DisconnectReasonServerShutdown = 1,

		/// <summary>
		/// The stream format changed for the device that the audio session is connected to.
		/// </summary>
		DisconnectReasonFormatChanged = 2,

		/// <summary>
		/// The user logged off the WTS session that the audio session was running in.
		/// </summary>
		DisconnectReasonSessionLogoff = 3,

		/// <summary>
		/// The WTS session that the audio session was running in was disconnected.
		/// </summary>
		DisconnectReasonSessionDisconnected = 4,

		/// <summary>
		/// The (shared-mode) audio session was disconnected to make the audio endpoint device available for an exclusive-mode connection.
		/// </summary>
		DisconnectReasonExclusiveModeOverride = 5
	}

	/// <summary>
	/// Windows CoreAudio IAudioSessionControl interface
	/// Defined in AudioPolicy.h
	/// </summary>
	[Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IAudioSessionEvents {
		/// <summary>
		/// Notifies the client that the display name for the session has changed.
		/// </summary>
		/// <param name="displayName">The new display name for the session.</param>
		/// <param name="eventContext">A user context value that is passed to the notification callback.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnDisplayNameChanged(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string displayName,
			[In] ref Guid eventContext);

		/// <summary>
		/// Notifies the client that the display icon for the session has changed.
		/// </summary>
		/// <param name="iconPath">The path for the new display icon for the session.</param>
		/// <param name="eventContext">A user context value that is passed to the notification callback.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnIconPathChanged(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string iconPath,
			[In] ref Guid eventContext);

		/// <summary>
		/// Notifies the client that the volume level or muting state of the session has changed.
		/// </summary>
		/// <param name="volume">The new volume level for the audio session.</param>
		/// <param name="isMuted">The new muting state.</param>
		/// <param name="eventContext">A user context value that is passed to the notification callback.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnSimpleVolumeChanged(
			[In] [MarshalAs(UnmanagedType.R4)] float volume,
			[In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
			[In] ref Guid eventContext);

		/// <summary>
		/// Notifies the client that the volume level of an audio channel in the session submix has changed.
		/// </summary>
		/// <param name="channelCount">The channel count.</param>
		/// <param name="newVolumes">An array of volumnes cooresponding with each channel index.</param>
		/// <param name="channelIndex">The number of the channel whose volume level changed.</param>
		/// <param name="eventContext">A user context value that is passed to the notification callback.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnChannelVolumeChanged(
			[In] [MarshalAs(UnmanagedType.U4)] uint channelCount,
			[In] [MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, // Pointer to float array
			[In] [MarshalAs(UnmanagedType.U4)] uint channelIndex,
			[In] ref Guid eventContext);

		/// <summary>
		/// Notifies the client that the grouping parameter for the session has changed.
		/// </summary>
		/// <param name="groupingId">The new grouping parameter for the session.</param>
		/// <param name="eventContext">A user context value that is passed to the notification callback.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnGroupingParamChanged(
			[In] ref Guid groupingId,
			[In] ref Guid eventContext);

		/// <summary>
		/// Notifies the client that the stream-activity state of the session has changed.
		/// </summary>
		/// <param name="state">The new session state.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnStateChanged(
			[In] AudioSessionState state);

		/// <summary>
		/// Notifies the client that the session has been disconnected.
		/// </summary>
		/// <param name="disconnectReason">The reason that the audio session was disconnected.</param>
		/// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
		[PreserveSig]
		int OnSessionDisconnected(
			[In] AudioSessionDisconnectReason disconnectReason);
	}
}