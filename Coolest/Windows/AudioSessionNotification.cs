using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	internal class AudioSessionNotification : IAudioSessionNotification {
		private AudioSessionManager parent;

		internal AudioSessionNotification(AudioSessionManager parent) {
			this.parent = parent;
		}

		[PreserveSig]
		public int OnSessionCreated(IAudioSessionControl newSession) {
			parent.FireSessionCreated(newSession);
			return 0;
		}
	}
}