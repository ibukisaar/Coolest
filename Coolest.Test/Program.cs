using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coolest.SoundOut.Windows;
using Coolest;
using System.Runtime.InteropServices;

namespace Coolest.Test {
	class Program {
		unsafe static void Main(string[] args) {
			WasapiOut wasapi = new WasapiOut(new TestStream(), ShareMode.Shared);
			wasapi.Play();

			Console.ReadKey();
		}
	}
}
