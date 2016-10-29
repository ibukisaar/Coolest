#pragma once

#include "AudioPlayer.h"

#ifdef COOLESTWINDOWS_EXPORTS
#define COOLEST_API __declspec(dllexport)
#else
#define COOLEST_API __declspec(dllimport)
#endif

extern "C" {
	COOLEST_API void GetErrorString(HRESULT errorCode, const char **outErrorString);

	COOLEST_API HRESULT CreateAudioPlayer(AudioPlayer **player, AUDCLNT_SHAREMODE shareMode, REFERENCE_TIME hnsRequestDuration, ERole role);
	COOLEST_API void ReleaseAudioPlayer(AudioPlayer *player);
	COOLEST_API HRESULT AudioPlayerInitialize(AudioPlayer * player, WAVEFORMATEX * waveFormat);
	COOLEST_API HRESULT AudioPlayerSetEventHandle(AudioPlayer *player, HANDLE eventHandle);
	COOLEST_API HRESULT AudioPlayerRequestFrameCount(AudioPlayer *player, UINT32 * outFramesCount, BOOL first);
	COOLEST_API HRESULT AudioPlayerSubmitBuffer(AudioPlayer * player, void *buffer, UINT32 framesCount);
	COOLEST_API HRESULT AudioPlayerStart(AudioPlayer *player);
	COOLEST_API HRESULT AudioPlayerStop(AudioPlayer *player);
	COOLEST_API HRESULT AudioPlayerIsFormatSupported(AudioPlayer *player, WAVEFORMATEX *waveFormat, WAVEFORMATEX **outClosestMatch);
	COOLEST_API HRESULT AudioPlayerGetMixFormat(AudioPlayer *player, WAVEFORMATEX **outFormat);
}