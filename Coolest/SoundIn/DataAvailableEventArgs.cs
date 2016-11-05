using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundIn {
	public class DataAvailableEventArgs : EventArgs {
		public byte[] Data { get; internal set; }
		public int Bytes { get; internal set; }

		internal DataAvailableEventArgs() { }

		public DataAvailableEventArgs(byte[] data, int bytes) {
			Data = data;
			Bytes = bytes;
		}
	}
}
