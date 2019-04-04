package com.Faizan.Github.BackgroundAudio;

public interface BackgroundAudioServiceCallback {
    void BackgroundAudioStarted();
    void BackgroundAudioStopped();
    void BackgroundAudioPaused();
    void BackgroundAudioResumed();
}
