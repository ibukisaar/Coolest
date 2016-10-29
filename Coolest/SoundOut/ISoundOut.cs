using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut {
	/// <summary>
	/// 音频输出接口
	/// </summary>
	public interface ISoundOut : IDisposable {
		void Play();
		void Stop();

	}
}
