using System;
using BackgroundAudio.Base;

namespace BackgroundAudio.UnsupportedPlatform
{
    public class UnsupportedPlatformBackgroundAudio : BackgroundAudioImplementation
    {
        public override event Action OnAudioStarted;
        public override event Action OnAudioStopped;
        public override event Action OnAudioPaused;
        public override event Action OnAudioResumed;

        protected override void Initialize()
        {
            //throw new PlatformNotSupportedException();
        }

        public override void Play(string path)
        {
           // throw new PlatformNotSupportedException();
        }

        public override void Stop()
        {
            //throw new PlatformNotSupportedException();
        }

        public override void Pause()
        {
            //throw new PlatformNotSupportedException();
        }

        public override void Resume()
        {
           // throw new PlatformNotSupportedException();
        }

        public override void Seek(float seconds)
        {
            //throw new PlatformNotSupportedException();
        }

        public override void SetVolume(float volume)
        {
           // throw new PlatformNotSupportedException();
        }

        public override void SetLoop(bool value)
        {
            //throw new PlatformNotSupportedException();
        }
        public override void SetSpeed(float speed)
        {
            //throw new NotImplementedException();
        }

        public override float GetCurrentPosition()
        {
            return 0;
            //throw new PlatformNotSupportedException();
        }

        public override float GetDuration()
        {
            return 0;

            //throw new PlatformNotSupportedException();
        }

        public override float GetVolume()
        {
            return 0;

           // throw new PlatformNotSupportedException();
        }

        public override bool IsLooping()
        {
            return false;

            throw new PlatformNotSupportedException();
        }

        public override bool IsPlaying()
        {
            return false;

            throw new PlatformNotSupportedException();
        }

        public override bool IsPaused()
        {
            return false;

            throw new PlatformNotSupportedException();
        }

        public override float GetSpeed()
        {
            return 1;

            throw new NotImplementedException();
        }
    }
}