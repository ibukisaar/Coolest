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
		R(S_OK, "�޴���");
		R(E_POINTER, "δ�ܻ����Ƶ�豸");
		R(E_INVALIDARG, "������Ч������Audio�ĳ�ʼ������");
		R(E_OUTOFMEMORY, "�ڴ����");
		R(AUDCLNT_E_ENDPOINT_CREATE_FAILED, "Audio Client����ʧ��");
		R(AUDCLNT_E_UNSUPPORTED_FORMAT, "��֧�ֵĸ�ʽ");
		R(AUDCLNT_E_BUFFER_SIZE_ERROR, "��ռģʽ���ύ��buffer��С��������");
	}

	IF(E_NOTFOUND, "δ�ҵ�֧�ֵ���Ƶ�豸");

	*outErrorMessage = "δ֪�Ĵ���";

#undef R
#undef IF
}