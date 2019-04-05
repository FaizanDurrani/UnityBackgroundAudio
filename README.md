# UnityBackgroundAudio
Play audio on mobile even when the game is suspended in the background

## Dependencies
The following plug-ins are needed for UnityBackgroundAudio to work.
- Unity-Swift - [Github](https://github.com/miyabi/unity-swift/releases)
- Unity-Jar-Resolver [Github](https://github.com/googlesamples/unity-jar-resolver/releases)

## Usage
Download and import the dependencies and the `.unitypackage` from [Releases](https://github.com/FaizanDurrani/UnityBackgroundAudio/releases)
```cs
// Get an instance of the BackgroundAudioImplementation class for the current build platform
var instance = BackgroundAudioManager.NewInstance();

...

// To play an mp3 file
// NOTE: Network playback currently not supported
instance.Play("/Path/To/File.mp3");

...

// Callbacks
// NOTE: Callbacks on Android are not invoked on the main thread (use a main thread dispatcher to update UI)
instance.OnAudioStarted += () => Debug.Log("Audio started playing");
instance.OnAudioStopped += () => Debug.Log("Audio stopped playing");
instance.OnAudioPaused += () => Debug.Log("Audio paused");
instance.OnAudioResumed += () => Debug.Log("Audio resumed");
```

## Caveats (Android)
#### Notification
The plugin starts a ForegroundService whenever an audio is played. This is required so that even if Android decides to kill the game the audio continues to play. To change the icon, simply replace the `ba_notification_icon.png` inside `Assets/Plugins/Android/res/drawable/` with your own icon.
#### Threading
`AndroidJNI.AttachCurrentThread();` must be called before calling any methods from outside the main thread and `AndroidJNI.DetachCurrentThread();` must be called before the thread closes.

## Sample
Clone or download the repo as `.zip` and open the `SampleScene` scene under `Assets/Scenes/`

## Notes
I am still learning Native Plugin development for Android and iOS so there might be inefficient/bad code in the `.java` or `.swift` files. Feel free to criticize or submit pull requests.
