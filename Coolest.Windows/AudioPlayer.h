#pragma once

#include <Windows.h>
#include <Audioclient.h>
#include <mmdeviceapi.h>

class AudioPlayer {
private:
	IAudioClient *audioClient;
	IAudioRenderClient *renderClient;
	UINT32 bufferFrameCount;
	// HANDLE eventObject;
	HRESULT currentErrorCode;
	AUDCLNT_SHAREMODE shareMode;
	REFERENCE_TIME requestDuration;
	WORD frameSize;

public:
	AudioPlayer(AUDCLNT_SHAREMODE shareMode, REFERENCE_TIME hnsRequestDuration, ERole role);
	~AudioPlayer();

	void Start();
	void Stop();
	void Initialize(WAVEFORMATEX * waveFormat);
	void RequestFrameCount(UINT32 *outFramesCount, BOOL first);
	void SubmitBuffer(void *buffer, UINT32 framesCount);
	void WaitRequest();
	void SetEventHandle(HANDLE eventHandle);
	HRESULT IsFormatSupported(WAVEFORMATEX *waveFormat, WAVEFORMATEX **outClosestMatch);
	HRESULT GetMixFormat(WAVEFORMATEX **outFormat);

	HRESULT GetCurrentErrorCode();
};