#include "AudioPlayer.h"

static const CLSID CLSID_MMDeviceEnumerator = __uuidof(MMDeviceEnumerator);
static const IID IID_IMMDeviceEnumerator = __uuidof(IMMDeviceEnumerator);
static const IID IID_IAudioClient = __uuidof(IAudioClient);
static const IID IID_IAudioRenderClient = __uuidof(IAudioRenderClient);

#define SAFE_RELEASE(punk) \
	if ((punk) != NULL) \
		{ (punk)->Release(); (punk) = NULL;}

#define TRY(OPERATE) \
{\
	currentErrorCode = (OPERATE);\
	if (FAILED(currentErrorCode)) goto Exit;\
}

AudioPlayer::AudioPlayer(AUDCLNT_SHAREMODE shareMode, REFERENCE_TIME hnsRequestDuration, ERole role)
	: audioClient(NULL), renderClient(NULL), bufferFrameCount(0), currentErrorCode(S_OK), shareMode(shareMode), requestDuration(hnsRequestDuration), frameSize(0) {
	IMMDeviceEnumerator* pEnumerator = NULL;
	IMMDevice* pDevice = NULL;

	TRY(CoCreateInstance(CLSID_MMDeviceEnumerator, NULL, CLSCTX_ALL, IID_IMMDeviceEnumerator, (void**) &pEnumerator));
	TRY(pEnumerator->GetDefaultAudioEndpoint(EDataFlow::eRender, role, &pDevice));
	TRY(pDevice->Activate(IID_IAudioClient, CLSCTX_ALL, NULL, (void **) &this->audioClient));

Exit:
	SAFE_RELEASE(pEnumerator);
	SAFE_RELEASE(pDevice);
}

AudioPlayer::~AudioPlayer() {
	this->Stop();
	SAFE_RELEASE(audioClient);
	SAFE_RELEASE(renderClient);
}

void AudioPlayer::Initialize(WAVEFORMATEX * waveFormat) {
	frameSize = waveFormat->nBlockAlign;
	TRY(audioClient->Initialize(shareMode, AUDCLNT_STREAMFLAGS_EVENTCALLBACK, requestDuration, shareMode == AUDCLNT_SHAREMODE_SHARED ? 0 : requestDuration, waveFormat, NULL));
	TRY(audioClient->GetBufferSize(&bufferFrameCount));
	TRY(audioClient->GetService(IID_IAudioRenderClient, (void **) &this->renderClient));

Exit:;
}

void AudioPlayer::Start() {
	TRY(audioClient->Start());
Exit:;
}

void AudioPlayer::SetEventHandle(HANDLE eventHandle) {
	TRY(audioClient->SetEventHandle(eventHandle));
Exit:;
}

void AudioPlayer::Stop() {
	audioClient->Stop();
}

HRESULT AudioPlayer::GetCurrentErrorCode() {
	return currentErrorCode;
}

void AudioPlayer::RequestFrameCount(UINT32 *outFramesCount, BOOL first) {
	if (shareMode == AUDCLNT_SHAREMODE_SHARED && !first) {
		UINT32 numFramesPadding;
		TRY(audioClient->GetCurrentPadding(&numFramesPadding));
		*outFramesCount = bufferFrameCount - numFramesPadding;
	} else {
		*outFramesCount = bufferFrameCount;
	}
Exit:;
}

void AudioPlayer::SubmitBuffer(void *buffer, UINT32 framesCount) {
	if (framesCount > 0) {
		BYTE *clientBuffer;
		TRY(renderClient->GetBuffer(framesCount, &clientBuffer));
		memcpy(clientBuffer, buffer, framesCount * frameSize);
		TRY(renderClient->ReleaseBuffer(framesCount, 0));
	}
Exit:;
}

HRESULT AudioPlayer::IsFormatSupported(WAVEFORMATEX *waveFormat, WAVEFORMATEX **outClosestMatch) {
	return audioClient->IsFormatSupported(shareMode, waveFormat, outClosestMatch);
}

HRESULT AudioPlayer::GetMixFormat(WAVEFORMATEX **outFormat) {
	return audioClient->GetMixFormat(outFormat);
}