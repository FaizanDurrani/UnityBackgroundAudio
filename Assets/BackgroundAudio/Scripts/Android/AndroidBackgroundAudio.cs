
using System;
using UnityEngine;
using BackgroundAudio.Base;
namespace BackgroundAudio.Android
{
    public class AndroidBackgroundAudio : BackgroundAudioImplementation
    {
        public const string CLASS_NAME = "BackgroundAudioService";
        public const string INTERFACE_NAME = "BackgroundAudioServiceCallback";
        public const string PACKAGE_NAME = "com.Faizan.Github.BackgroundAudio";
    
        private const int SECONDS_TO_MILLISECONDS = 1000;
        private const float MILLISECONDS_TO_SECONDS = 0.001f;
    
        private static volatile AndroidJavaClass _service;
        private static volatile AndroidJavaObject _unityActivity;
    
        private AndroidBackgroundAudioCallback _callbackListener;
        private bool _playing, _paused, _looping;
        private float _volume = 1;
    
        public override event Action OnAudioStarted, OnAudioStopped, OnAudioPaused, OnAudioResumed;
    
        protected override void Initialize()
        {
            Application.runInBackground = true;
            if (_unityActivity == null)
            {
                AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
    
            if (_service == null)
                _service = new AndroidJavaClass(PACKAGE_NAME + "." + CLASS_NAME);
    
            _callbackListener = new AndroidBackgroundAudioCallback();
            _callbackListener.OnAudioStarted += () =>
            {
                _playing = true;
                _paused = false;
                SetLoop(_looping);
                SetVolume(_volume);
                
                OnAudioStarted?.Invoke();
            };
    
            _callbackListener.OnAudioStopped += () =>
            {
                _playing = false;
                _paused = false;
                OnAudioStopped?.Invoke();
            };
    
            _callbackListener.OnAudioResumed += () =>
            {
                _playing = true;
                _paused = false;
                OnAudioResumed?.Invoke();
            };
    
            _callbackListener.OnAudioPaused += () =>
            {
                _playing = false;
                _paused = true;
                OnAudioPaused?.Invoke();
            };
    
            CallOnService("initialize", Id, _callbackListener);
        }
    
        ~AndroidBackgroundAudio()
        {
            CallOnService("uninitialize", Id);
            _callbackListener = null;
    
            if (IsPlaying())
            {
                Stop();
            }
        }
    
        public override void Play(string path)
        {
            CallOnService("play", _unityActivity, Id, path, Application.identifier);
        }
    
        public override void Pause()
        {
            if (!IsPlaying()) return;
    
            CallOnService("pause", _unityActivity, Id);
        }
    
        public override void Resume()
        {
            if (IsPlaying()) return;
    
            CallOnService("resume", _unityActivity, Id);
        }
    
        public override void Seek(float seconds)
        {
            if (!_playing && !_paused) return;
    
            CallOnService("seek", _unityActivity, Id, (int) (seconds * SECONDS_TO_MILLISECONDS));
        }
    
        public override float GetVolume()
        {
            return _volume;
        }
    
        public override bool IsLooping()
        {
            return _looping;
        }
    
        public override bool IsPlaying()
        {
            return _playing && !_paused && (_playing = CallOnService<bool>("isPlaying", Id));
        }
    
        public override bool IsPaused()
        {
            return _paused && (_paused = CallOnService<bool>("isPaused", Id));
        }
    
        public override float GetCurrentPosition()
        {
            return _playing || _paused ? CallOnService<int>("getCurrentPosition", Id) * MILLISECONDS_TO_SECONDS : 0;
        }
    
        public override float GetDuration()
        {
            return _playing || _paused ? CallOnService<int>("getDuration", Id) * MILLISECONDS_TO_SECONDS : 0;
        }
    
        public override void Stop()
        {
            if (!_paused && !_playing) return;
    
            CallOnService("disposeInstance", _unityActivity, Id);
        }
    
        public override void SetVolume(float volume)
        {
            _volume = volume;
            if (!_playing && !_paused) return;
    
            CallOnService("setVolume", _unityActivity, Id, volume);
        }
    
        public override void SetLoop(bool value)
        {
            _looping = value;
            if (!_playing && !_paused) return;
    
            CallOnService("setLoop", _unityActivity, Id, value);
        }
    
        private static void CallOnService(string method, params object[] args)
        {
            _service.CallStatic(method, args);
        }
    
        private static T CallOnService<T>(string method, params object[] args)
        {
            return _service.CallStatic<T>(method, args);
        }
    }
}