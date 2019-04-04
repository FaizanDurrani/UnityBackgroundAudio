using System;
using System.Runtime.InteropServices;
using BackgroundAudio.Base;

namespace BackgroundAudio.IOS
{
    public class IOSBackgroundAudio : BackgroundAudioImplementation
    {
        private static int _globalCounter;
        private bool _playing, _paused, _looping;
        private float _volume = 1;
    
        [DllImport("__Internal")]
        private static extern void _initialize(int id);
    
        [DllImport("__Internal")]
        private static extern void _play(int id, string path);
    
        [DllImport("__Internal")]
        private static extern void _resume(int id);
    
        [DllImport("__Internal")]
        private static extern void _pause(int id);
    
        [DllImport("__Internal")]
        private static extern void _stop(int id);
    
        [DllImport("__Internal")]
        private static extern void _seek(int id, float seconds);
    
        [DllImport("__Internal")]
        private static extern float _getCurrentPosition(int id);
    
        [DllImport("__Internal")]
        private static extern float _getDuration(int id);
    
        [DllImport("__Internal")]
        private static extern void _setVolume(int id, float volume);
    
        [DllImport("__Internal")]
        private static extern void _setLoop(int id, bool value);
    
        [DllImport("__Internal")]
        private static extern bool _isLooping(int id);
    
        [DllImport("__Internal")]
        private static extern bool _isPlaying(int id);
    
        public override event Action OnAudioStarted;
        public override event Action OnAudioStopped;
        public override event Action OnAudioPaused;
        public override event Action OnAudioResumed;
    
        protected override void Initialize()
        {
            _initialize(Id);
        }
    
        public override void Play(string path)
        {
            _play(Id, path);
            _setLoop(Id, _looping);
            _setVolume(Id, _volume);
            _playing = true;
            _paused = false;
            OnAudioStarted?.Invoke();
        }
    
        public override void Stop()
        {
            if (!IsPlaying()) return;
    
            _stop(Id);
            _playing = false;
            _paused = false;
            OnAudioStopped?.Invoke();
        }
    
        public override void Pause()
        {
            if (!IsPlaying()) return;
    
            _pause(Id);
            _playing = false;
            _paused = true;
            OnAudioPaused?.Invoke();
        }
    
        public override void Resume()
        {
            if (IsPlaying()) return;
    
            _resume(Id);
            _playing = true;
            _paused = false;
            OnAudioResumed?.Invoke();
        }
    
        public override void Seek(float seconds)
        {
            if (!_playing && !_paused) return;
    
            _seek(Id, seconds);
        }
    
        public override float GetCurrentPosition()
        {
            return _playing || _paused ? _getCurrentPosition(Id) : 0;
        }
    
        public override float GetDuration()
        {
            return _playing || _paused ? _getDuration(Id) : 0;
        }
    
        public override float GetVolume()
        {
            return _volume;
        }
    
        public override void SetVolume(float volume)
        {
            _volume = volume;
            if (!_playing && !_paused) return;
    
            _setVolume(Id, volume);
        }
    
        public override void SetLoop(bool value)
        {
            _looping = value;
            if (!IsPlaying()) return;
    
            _setLoop(Id, value);
        }
    
        public override bool IsLooping()
        {
            return IsPlaying() && _looping && (_looping = _isLooping(Id));
        }
    
        public override bool IsPlaying()
        {
            return _playing && !_paused && (_playing = _isPlaying(Id));
        }
    
        public override bool IsPaused()
        {
            return _paused;
        }
    }
}
