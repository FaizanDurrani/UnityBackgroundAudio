using System;
using System.IO;
using BackgroundAudio;
using BackgroundAudio.Base;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private BackgroundAudioImplementation _androidBackgroundAudio;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Slider _seekBar;
    [SerializeField] private Text _speedTex;
    [SerializeField] private Slider _speedBar;

    private void Awake()
    {
        _androidBackgroundAudio = BackgroundAudioManager.NewInstance();

        _androidBackgroundAudio.OnAudioStarted += () => UnityMainThreadDispatcher.Instance().Enqueue(() => _seekBar.maxValue = _androidBackgroundAudio.GetDuration());
        _toggle.onValueChanged.AddListener(SetLooping);

    }
    private void Start()
    {
        _speedBar.value=(_androidBackgroundAudio.GetSpeed()/0.25f);
    }

    public void Play()
    {
        GetAudioFileURI(s => _androidBackgroundAudio.Play(s));
    }

    private void GetAudioFileURI(Action<string> callback)
    {
#if UNITY_IOS // IOS can read from StreamingAssets

        var filePath = Path.Combine(Application.streamingAssetsPath, "SampleAudio.mp3");
        callback?.Invoke(filePath);
        return;

#endif
#if UNITY_ANDROID // Android can't
        var persistantPath = Path.Combine(Application.persistentDataPath, "SampleAudio.mp3");
        var filePath = Path.Combine(Application.streamingAssetsPath, "SampleAudio.mp3");
        Debug.Log($"PersistantPath: {persistantPath}");
        if (File.Exists(persistantPath))
        {
            Debug.Log("Exists");
            callback?.Invoke(persistantPath);
            return;
        }

        Debug.Log($"StreamingPath: {filePath}");

        var req = new UnityWebRequest(filePath, UnityWebRequest.kHttpVerbGET, new DownloadHandlerFile(persistantPath), null);
        var asyncOp = req.SendWebRequest();
        asyncOp.completed += op => { callback?.Invoke(persistantPath); };
#endif
    }

    private void Update()
    {
        if (_androidBackgroundAudio.IsPlaying())
        {
            _seekBar.value = _androidBackgroundAudio.GetCurrentPosition();
        }
    }

    public void Pause()
    {
        _androidBackgroundAudio.Pause();
    }

    public void Resume()
    {
        _androidBackgroundAudio.Resume();
    }

    public void SeekFwd()
    {
        _androidBackgroundAudio.Seek(5f);
    }

    public void SeekBwd()
    {
        _androidBackgroundAudio.Seek(-5f);
    }

    public void SetLooping(bool value)
    {
        _androidBackgroundAudio.SetLoop(value);
    }

    public void SetSpeed(float value)
    {
        var speed = value * 0.25f;
        _speedTex.text = speed.ToString();
        _androidBackgroundAudio.SetSpeed(speed);
    }
}