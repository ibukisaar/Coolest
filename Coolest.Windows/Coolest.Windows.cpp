#include "Coolest.Windows.h"

COOLEST_API HRESULT CreateAudioPlayer(AudioPlayer ** player, AUDCLNT_SHAREMODE shareMode, REFERENCE_TIME hnsRequestDuration, ERole role) {
	*player = new AudioPlayer { shareMode, hnsRequestDuration, role };
	return (*player)->GetCurrentErrorCode();
}

COOLEST_API void ReleaseAudioPlayer(AudioPlayer * player) {
	delete player;
}

COOLEST_API HRESULT AudioPlayerInitialize(AudioPlayer * player, WAVEFORMATEX * waveFormat) {
	player->Initialize(waveFormat);
	return player->GetCurrentErrorCode();
}

COOLEST_API HRESULT AudioPlayerSetEventHandle(AudioPlayer *player, HANDLE eventHandle) {
	player->SetEventHandle(eventHandle);
	return player->GetCurrentErrorCode();
}

COOLEST_API HRESULT AudioPlayerRequestFrameCount(AudioPlayer *player, UINT32 * outFramesCount, BOOL first) {
	player->RequestFrameCount(outFramesCount, first);
	return player->GetCurrentErrorCode();
}

COOLEST_API HRESULT AudioPlayerSubmitBuffer(AudioPlayer * player, void *buffer, UINT32 framesCount) {
	player->SubmitBuffer(buffer, framesCount);
	return player->GetCurrentErrorCode();
}

COOLEST_API HRESULT AudioPlayerIsFormatSupported(AudioPlayer *player, WAVEFORMATEX *waveFormat, WAVEFORMATEX **outClosestMatch) {
	return player->IsFormatSupported(waveFormat, outClosestMatch);
}

COOLEST_API HRESULT AudioPlayerGetMixFormat(AudioPlayer *player, WAVEFORMATEX **outFormat) {
	return player->GetMixFormat(outFormat);
}

COOLEST_API HRESULT AudioPlayerStart(AudioPlayer *player) {
	player->Start();
	return player->GetCurrentErrorCode();
}

COOLEST_API HRESULT AudioPlayerStop(AudioPlayer *player) {
	player->Stop();
	return player->GetCurrentErrorCode();
}

COOLEST_API void GetErrorString(HRESULT errorCode, const char **outErrorMessage) {
#define R(CODE, MESSAGE) case CODE: *outErrorMessage = MESSAGE; return
#define IF(CODE, MESSAGE) do { if (errorCode == (CODE)) { *outErrorMessage = MESSAGE; return; } } while (false)

	switch (errorCode) {
		R(S_OK, "无错误");
		R(E_POINTER, "未能获得音频设备");
		R(E_INVALIDARG, "参数无效，请检查Audio的初始化参数");
		R(E_OUTOFMEMORY, "内存溢出");
		R(AUDCLNT_E_ENDPOINT_CREATE_FAILED, "Audio Client创建失败");
		R(AUDCLNT_E_UNSUPPORTED_FORMAT, "不支持的格式");
		R(AUDCLNT_E_BUFFER_SIZE_ERROR, "独占模式下提交的buffer大小发生错误");
	}

	IF(E_NOTFOUND, "未找到支持的音频设备");

	*outErrorMessage = "未知的错误";

#undef R
#undef IF
}