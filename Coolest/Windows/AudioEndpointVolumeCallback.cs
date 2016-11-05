using Coolest.Windows.ComInterface;
using System;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	// This class implements the IAudioEndpointVolumeCallback interface,
	// it is implemented in this class because implementing it on AudioEndpointVolume 
	// (where the functionality is really wanted, would cause the OnNotify function 
	// to show up in the public API. 
	internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback {
		private readonly AudioEndpointVolume parent;

		internal AudioEndpointVolumeCallback(AudioEndpointVolume parent) {
			this.parent = parent;
		}

		public void OnNotify(IntPtr notifyData) {
			//Since AUDIO_VOLUME_NOTIFICATION_DATA is dynamic in length based on the
			//number of audio channels available we cannot just call PtrToStructure 
			//to get all data, thats why it is split up into two steps, first the static
			//data is marshalled into the data structure, then with some IntPtr math the
			//remaining floats are read from memory.
			//
			var data = Marshal.PtrToStructure<AudioVolumeNotificationDataStruct>(notifyData);

			//Determine offset in structure of the first float
			var offset = Marshal.OffsetOf<AudioVolumeNotificationDataStruct>(nameof(AudioVolumeNotificationDataStruct.ChannelVolume));
			//Determine offset in memory of the first float
			var firstFloatPtr = (IntPtr) ((long) notifyData + (long) offset);

			var voldata = new float[data.Channels];

			//Read all floats from memory.
			for (int i = 0; i < data.Channels; i++) {
				voldata[i] = Marshal.PtrToStructure<float>(firstFloatPtr);
			}

			//Create combined structure and Fire Event in parent class.
			var notificationData = new AudioVolumeNotificationData(data.GuidEventContext, data.Muted, data.MasterVolume, voldata, data.GuidEventContext);
			parent.FireNotification(notificationData);
		}
	}
}