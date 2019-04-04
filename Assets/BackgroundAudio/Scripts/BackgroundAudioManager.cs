using BackgroundAudio.Base;
#if UNITY_EDITOR || UNITY_STANDALONE
using BackgroundAudio.UnsupportedPlatform;

#elif UNITY_ANDROID
    using BackgroundAudio.Android;
#elif UNITY_IPHONE || UNITY_IOS
    using BackgroundAudio.IOS;
#endif

namespace BackgroundAudio
{
    public static class BackgroundAudioManager
    {
        public static BackgroundAudioImplementation NewInstance()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            return new UnsupportedPlatformBackgroundAudio();
            #elif UNITY_ANDROID
            return new AndroidBackgroundAudio();
            #elif UNITY_IPHONE || UNITY_IOS
            return new IOSBackgroundAudio();
            #endif
        }
    }
}