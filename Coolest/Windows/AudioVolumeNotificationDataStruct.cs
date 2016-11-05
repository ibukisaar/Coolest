using System;

namespace Coolest.Windows {
	internal struct AudioVolumeNotificationDataStruct {
		public Guid GuidEventContext { get; set; }
		public bool Muted { get; set; }
		public float MasterVolume { get; set; }
		public uint Channels { get; set; }
		public float ChannelVolume { get; set; }
	}
}