using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest {
	public interface IWaveStream {
		/// <summary>
		/// 音源格式
		/// </summary>
		WaveFormat Format { get; }
		/// <summary>
		/// 通知音源写入缓冲区
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		int Read(byte[] buffer, int start, int length);
	}
}
