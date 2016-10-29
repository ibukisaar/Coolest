using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.SoundOut.Windows {
	/// <summary>
	/// 设置共享模式或独占模式
	/// </summary>
	public enum ShareMode {
		/// <summary>
		/// 共享模式，与其他应用程序共享音频输出。
		/// <para>该模式的支持格式可能会劣于 <see cref="Exclusive"/> 。</para>
		/// </summary>
		Shared,
		/// <summary>
		/// 独占模式，会强制阻断其他应用程序的音频输出。
		/// <para>如果高音质音源（>48000 Hz），则建议使用该模式。该模式的支持格式与硬件相关，使用前请查阅硬件支持的格式。</para>
		/// </summary>
		Exclusive
	}
}
