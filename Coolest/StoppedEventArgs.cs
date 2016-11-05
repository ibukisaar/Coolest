using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest {
	public class StoppedEventArgs : EventArgs {
		private Exception exception;

		public StoppedEventArgs(Exception exception = null) {
			this.exception = exception;
		}

		public bool HasError => exception != null;

		public Exception Exception => exception;
	}
}
