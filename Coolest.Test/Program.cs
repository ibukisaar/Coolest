using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Coolest;
using System.Runtime.InteropServices;
using Coolest.Windows;

namespace Coolest.Test {
	class Program {
		unsafe static void Main(string[] args) {
			WasapiOut wasapi = new WasapiOut();
			wasapi.Initialize(new TestStream());
			wasapi.Play();

			Console.ReadKey();
		}
	}
}
