

using System;
using UnityEngine;

namespace BackgroundAudio.Android{
    public class AndroidBackgroundAudioCallback : AndroidJavaProxy
    {
        public event Action OnAudioStarted, OnAudioStopped, OnAudioPaused, OnAudioResumed;
    
        public AndroidBackgroundAudioCallback() : base(AndroidBackgroundAudio.PACKAGE_NAME + "." + AndroidBackgroundAudio.INTERFACE_NAME)
        {
        }
    
        private void BackgroundAudioStarted()
        {
            OnAudioStarted?.Invoke();
        }
    
        private void BackgroundAudioStopped()
        {
            OnAudioStopped?.Invoke();
        }
    
        private void BackgroundAudioPaused()
        {
            OnAudioPaused?.Invoke();
        }
    
        private void BackgroundAudioResumed()
        {
            OnAudioResumed?.Invoke();
        }
    }

}