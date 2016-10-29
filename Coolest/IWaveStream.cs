using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest {
	public interface IWaveStream {
		/// <summary>
		/// （必须实现）音源格式
		/// </summary>
		WaveFormat Format { get; }
		/// <summary>
		/// （必须实现）通知音源写入缓冲区
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		int Read(byte[] buffer, int start, int length);
		/// <summary>
		/// 当播放设备不支持音源格式，则会通知音源进行重采样
		/// </summary>
		/// <param name="newFormat"></param>
		void Resample(WaveFormat newFormat);
	}
}
