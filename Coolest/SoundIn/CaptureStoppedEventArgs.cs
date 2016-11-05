using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundIn {
	public class CaptureStoppedEventArgs : StoppedEventArgs {
		public new static CaptureStoppedEventArgs Empty { get; } = new CaptureStoppedEventArgs();

		public CaptureStoppedEventArgs(Exception e = null) : base(e) { }
	}
}
