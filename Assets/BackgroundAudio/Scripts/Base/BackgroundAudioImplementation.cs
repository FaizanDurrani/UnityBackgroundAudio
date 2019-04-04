using System;

namespace BackgroundAudio.Base
{
    public abstract class BackgroundAudioImplementation
    {
        public abstract event Action OnAudioStarted, OnAudioStopped, OnAudioPaused, OnAudioResumed;
        private static int _globalCounter;
        public readonly int Id;

        protected BackgroundAudioImplementation()
        {
            Id = _globalCounter++;
            Initialize();
        }

        protected abstract void Initialize();
        public abstract void Play(string path);
        public abstract void Stop();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Seek(float seconds);

        public abstract void SetVolume(float volume);
        public abstract void SetLoop(bool value);

        public abstract float GetCurrentPosition();
        public abstract float GetDuration();
        public abstract float GetVolume();

        public abstract bool IsLooping();
        public abstract bool IsPlaying();
        public abstract bool IsPaused();
    }
}