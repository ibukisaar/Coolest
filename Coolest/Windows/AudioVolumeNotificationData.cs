using System;

namespace Coolest.Windows {
	internal class AudioVolumeNotificationData {
		private readonly Guid eventContext;
		private readonly bool muted;
		private readonly float masterVolume;
		private readonly int channels;
		private readonly float[] channelVolume;
		private readonly Guid guid;

		/// <summary>
		/// Event Context
		/// </summary>
		public Guid EventContext => eventContext;

		/// <summary>
		/// Muted
		/// </summary>
		public bool Muted => muted;

		/// <summary>
		/// Guid that raised the event
		/// </summary>
		public Guid Guid => guid;

		/// <summary>
		/// Master Volume
		/// </summary>
		public float MasterVolume => masterVolume;

		/// <summary>
		/// Channels
		/// </summary>
		public int Channels => channels;

		/// <summary>
		/// Channel Volume
		/// </summary>
		public float[] ChannelVolume => channelVolume;

		/// <summary>
		/// Audio Volume Notification Data
		/// </summary>
		/// <param name="eventContext"></param>
		/// <param name="muted"></param>
		/// <param name="masterVolume"></param>
		/// <param name="channelVolume"></param>
		/// <param name="guid"></param>
		public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume, Guid guid) {
			this.eventContext = eventContext;
			this.muted = muted;
			this.masterVolume = masterVolume;
			channels = channelVolume.Length;
			this.channelVolume = channelVolume;
			this.guid = guid;
		}
	}
}