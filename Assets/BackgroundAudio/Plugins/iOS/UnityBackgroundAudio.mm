
#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
// This header file is generated automatically when Xcode build runs.
#import "unityswift-Swift.h"    // Required

extern "C" {
    
    void _initialize (const int identifier) {
        [UnityBackgroundAudio initialize: identifier];
    }
    
    void _play(const int identifier, const char *path) {
        [UnityBackgroundAudio playOnInstance: identifier fromUrl: [NSString stringWithUTF8String: path]];
    }
    
    void _resume (const int identifier) {
        [UnityBackgroundAudio resumeWithInstanceId:identifier];
    }
    
    void _pause (const int identifier) {
        [UnityBackgroundAudio pauseWithInstanceId:identifier];
    }
    
    void _stop(const int identifier) {
        [UnityBackgroundAudio disposeWithInstanceId:identifier];
    }
    
    void _seek(const int identifier, const float seconds){
        [UnityBackgroundAudio seekWithInstanceId:identifier forSeconds:seconds];
    }
    
    float _getCurrentPosition(const int identifier) {
        return [UnityBackgroundAudio getCurrentPositionForInstanceId:identifier];
    }
    
    float _getDuration(const int identifier) {
        return [UnityBackgroundAudio getDurationForInstanceId:identifier];
    }
    
    void _setVolume(const int identifier, const float volume) {
        [UnityBackgroundAudio setVolumeForInstanceId:identifier to:volume];
    }
    
    void _setLoop(const int identifier, const bool value) {
        [UnityBackgroundAudio setLoopForInstanceId:identifier to:value];
    }
    
    
    bool _isLooping(const int identifier) {
        return [UnityBackgroundAudio isLoopingOnInstanceId:identifier];
    }
    
    bool _isPlaying(const int identifier) {
        return [UnityBackgroundAudio isPlayingOnInstanceId:identifier];
    }
    
}
