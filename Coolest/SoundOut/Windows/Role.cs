namespace Coolest.SoundOut.Windows {
	/// <summary>
	/// 指定一个应用场景，让系统更好的管理音量
	/// </summary>
	public enum Role {
		/// <summary>
		/// 游戏，系统通知声音，语音命令
		/// </summary>
		Console = 0,

		/// <summary>
		/// 音乐，视频，实时录制
		/// </summary>
		Multimedia,

		/// <summary>
		/// 语音通话（P2P）
		/// </summary>
		Communications
	}
}