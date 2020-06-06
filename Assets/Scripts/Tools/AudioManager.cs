using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;
 

public enum AudioGroupType
{
    Default,
    Bgm,
    Effect,
    Count,
}

[LabelText("音频配置")]
public class AudioClipData
{
    //[InlineEditor(InlineEditorModes.SmallPreview)]
    //[LabelText("音效文件")]
    //public AudioClip AudioClip;

    [ResourcePath(typeof(AudioClip))]
    [LabelText("音效资源")]
    public string AudioRes;

    [LabelText("音量大小")]
    public float Volume = 1;
}



public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioMixer Mixer;
    public AudioMixerGroup[] MixerGroups= new AudioMixerGroup[3];
 
    public class AudioHandle
    {
        public uint HandleId;
        public float StartTime;
        public float FinishTime;
        public Transform PosNode;
        public AudioClip Clip;
        public AudioSource Source;
        public bool Loop;
        public float Volume;
        public float ClipLength;
    }


    public const int PreAllocSourceCount = 10;
    public const int MaxSoundCount = 64;

    public Transform AudioListenerNode;

    public Transform AudioNode;
    public AudioSource BgmSource;
    public string CurrentBgm;

    private Tween _volumeTween;

    public bool AudioEnable
    {
        get { return AudioNode.gameObject.activeSelf; }
        set
        {
            if (AudioNode == null) return;
            AudioNode.gameObject.SetActive(value);
        }
    }

    public bool BgmEnable
    {
        get { return BgmSource.gameObject.activeSelf; }
        set
        {
            if (BgmSource == null) return;

            BgmSource.gameObject.SetActive(value);
            if (value && BgmSource.clip != null && !BgmSource.isPlaying && BgmSource.loop)
            {
                BgmSource.Play();
            }
        }
    }

    public float BgmVolume
    {
        get { return _bgmVolume; }
        set
        {
            _bgmVolume = value;
            Mixer.SetFloat("BgmVolume", VolumeToDb(_bgmVolume));
        }
    }
    public float SoundVolume
    {
        get { return _soundVolume; }
        set
        {
            _soundVolume = value;
            Mixer.SetFloat("SoundVolume", VolumeToDb(_soundVolume));
        }
    }

    public const float MinVolumeDb = -20.0f;
    public const float MaxVolumeDb = 0;

    public float SourceFadeDistance = 20;
    public float MaxSourceDistance = 30;


    private readonly Stack<AudioSource> _freeSoundSourceList = new Stack<AudioSource>();
    private readonly List<AudioHandle> _usingSoundList = new List<AudioHandle>();
    private uint _soundHandleId = 0;

    private readonly Dictionary<string, AudioClip> _audioClipDict = new Dictionary<string, AudioClip>();

    private float _bgmVolume = 1.0f;
    private float _soundVolume = 1.0f;




    public static float VolumeToDb(float volume)
    {
        return volume == 0f ? -80f : MinVolumeDb + (MaxVolumeDb - MinVolumeDb) * volume;
    }

    public void Clear()
    {
        _audioClipDict.Clear();
    }

    protected override void Initialize()
    {
        MaxSourceDistance = Mathf.Max(SourceFadeDistance, MaxSourceDistance);
        _bgmVolume = 1.0f;
        _soundVolume = 1.0f;
        Mixer.SetFloat("BgmVolume", _bgmVolume);
        Mixer.SetFloat("SoundVolume", _soundVolume);

        for (int i = 0; i < PreAllocSourceCount; i++)
        {
            var go = new GameObject();
            go.transform.SetParent(AudioNode, false);
            var source = go.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.playOnAwake = false;
            _freeSoundSourceList.Push(source);
        }
    }

    public AudioClip GetAudioClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            return null;
        }
        AudioClip clipHandle;
        if (!_audioClipDict.TryGetValue(clipName, out clipHandle))
        {
            clipHandle = Resources.Load<AudioClip>("Sound/" + clipName);

            _audioClipDict[clipName] = clipHandle;
            return clipHandle;
        }
        if (clipHandle != null)
        {
            return clipHandle;
        }

        return null;
    }


    public float GetVolume(float dist)
    {
        if (dist >= MaxSourceDistance)
        {
            return 0;
        }
        if (dist <= SourceFadeDistance)
        {
            return 1.0f;
        }
        return (MaxSourceDistance - dist) / (MaxSourceDistance - SourceFadeDistance);
    }

    #region Bgm
    public void SetBgm(string clipName)
    {
        var clip = GetAudioClip(clipName);
        if (clip == null)
        {
            BgmSource.Stop();
            BgmSource.clip = null;
        }
        else
        {
            BgmSource.clip = clip;
            BgmSource.loop = true;
            BgmSource.volume = _bgmVolume;
            BgmSource.Play();
        }
    }

    public void SetBgm(AudioClipData data)
    {
        if (data == null)
        {
            BgmSource.Stop();
            BgmSource.clip = null;
            CurrentBgm = "";
        }
        else
        {
            CurrentBgm = data.AudioRes;
            BgmSource.clip = Resources.Load<AudioClip>(data.AudioRes);
            BgmSource.loop = true;
            BgmSource.volume = _bgmVolume * data.Volume;
            BgmSource.Play();
        }
    }

    /// <summary>
    /// 背景音乐渐入
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeTime"></param>
    public void SetBgmFadeIn(string clipName, float fadeTime)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            SetBgmFadeOut(fadeTime);
            return;
        }

        SetBgm(clipName);

        if (BgmSource.clip != null)
        {
            if (_volumeTween != null)
            {
                _volumeTween.Kill();
            }

            BgmSource.volume = 0;
            _volumeTween = DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, _bgmVolume, fadeTime).OnComplete(() =>
             {
                 _volumeTween = null;
             });
        }
    }



    /// <summary>
    /// 背景音乐渐入
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeTime"></param>
    public void SetBgmFadeIn(AudioClipData data, float fadeTime)
    {
        if (data.AudioRes == CurrentBgm) 
        {
            return;
        }

        if (data==null)
        {
            SetBgmFadeOut(fadeTime);
            return;
        }

        SetBgm(data);

        if (BgmSource.clip != null)
        {
            if (_volumeTween != null)
            {
                _volumeTween.Kill();
            }

            BgmSource.volume = 0;
            _volumeTween = DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, _bgmVolume* data.Volume, fadeTime).OnComplete(() =>
            {
                _volumeTween = null;
            });
        }
    }

    /// <summary>
    /// 背景音乐渐出
    /// </summary>
    public void SetBgmFadeOut(float fadeTime)
    {
        if (BgmSource.clip != null)
        {
            if (_volumeTween != null)
            {
                _volumeTween.Kill();
            }

            _volumeTween = DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, 0, fadeTime).OnComplete(() =>
            {
                CurrentBgm = "";
                BgmSource.Stop();
                BgmSource.clip = null;
            });
        }
    }
    /// <summary>
    /// 背景音乐渐入渐出
    /// </summary>
    public void SetBgmFadeInOut(string clipName, float fadeTime)
    {
        if (BgmSource.clip != null)
        {
            if (_volumeTween != null)
            {
                _volumeTween.Kill();
            }

            _volumeTween = DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, 0, fadeTime).
            OnComplete(() =>
            {
                BgmSource.Stop();
                BgmSource.clip = null;
                SetBgmFadeIn(clipName, fadeTime);
            });
        }
        else
        {
            SetBgmFadeIn(clipName, fadeTime);
        }
    }

    /// <summary>
    /// 背景音乐渐入渐出
    /// </summary>
    public void SetBgmFadeInOut(AudioClipData data, float fadeTime)
    {
        if (BgmSource.clip != null)
        {
            if (_volumeTween != null)
            {
                _volumeTween.Kill();
            }

            _volumeTween = DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, 0, fadeTime).
            OnComplete(() =>
            {
                BgmSource.Stop();
                BgmSource.clip = null;
                SetBgmFadeIn(data, fadeTime);
            });
        }
        else
        {
            SetBgmFadeIn(data, fadeTime);
        }
    }


    public void SetBgmSpeed(float speed)
    {
        BgmSource.pitch = speed;
        BgmSource.volume = BgmSource.pitch;
    }

 
    #endregion Bgm

    #region Sound


    AudioSource AllocSoundSource()
    {
        if (_freeSoundSourceList.Count > 0)
        {
            var source = _freeSoundSourceList.Pop();
            return source;
        }
        var totalCount = _freeSoundSourceList.Count + _usingSoundList.Count;
        if (totalCount < MaxSoundCount)
        {
            var go = new GameObject();
            go.transform.SetParent(AudioNode, false);
            var source = go.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.playOnAwake = false;
            return source;
        }
        else
        {
            return null;
        }
    }

    public float PlaySound(AudioClipData data, AudioGroupType group, Action<float> OnFinishTime = null)
    {
        if (data == null||string.IsNullOrEmpty(data.AudioRes))
        {
            return 0;
        }
        var clip = Resources.Load<AudioClip>(data.AudioRes);
        PlaySound(clip, null, AudioListenerNode.position, false, data.Volume, group, OnFinishTime);
        return clip.length;
    }

 
    public uint PlaySound(AudioClip clip, Transform posNode, Vector3 pos, bool loop, float volume, AudioGroupType group, Action<float> OnFinishTime = null)
    {
        if (clip==null)
        {
            return 0;
        }
        if (!loop && !AudioNode.gameObject.activeSelf)
        {
            return 0;
        }
        if (posNode != null)
        {
            pos = posNode.transform.position;
        }
        pos.z = AudioListenerNode.position.z;
        var dist2 = (AudioListenerNode.position - pos).sqrMagnitude;
        if (!loop && dist2 > MaxSourceDistance * MaxSourceDistance)
        {
            return 0;
        }
        var source = AllocSoundSource();
        if (source == null)
        {
            return 0;
        }

        AudioClip clipHandle = clip;

        if (clipHandle == null)
        {
            _freeSoundSourceList.Push(source);
            return 0;
        }


        source.outputAudioMixerGroup = MixerGroups[(int)group];
        var handle = ObjectPool<AudioHandle>.DefaultAlloc();
        handle.HandleId = ++_soundHandleId;
        handle.StartTime = Time.realtimeSinceStartup;
        handle.Source = source;
        handle.PosNode = posNode;
        handle.Loop = loop;
        handle.Volume = volume;

        source.transform.position = pos;
        _usingSoundList.Add(handle);
        source.volume = _soundVolume <= 0 ? _soundVolume : _soundVolume * volume * GetVolume(Mathf.Sqrt(dist2));
        source.loop = loop;
        handle.Clip = clipHandle;
        if (OnFinishTime != null)
        {
            OnFinishTime(handle.ClipLength);
        }
        return handle.HandleId;
    }


    void PlayHandle(AudioHandle handle, AudioClip clip, float dt, Action<float> OnFinishTime = null)
    {
        if (handle.Loop)
        {
            handle.Source.clip = clip;
            handle.Source.loop = true;
            if (AudioNode.gameObject.activeSelf)
            {
                handle.Source.Play();
            }
        }
        else
        {
            handle.ClipLength = clip.length;
            handle.FinishTime = handle.StartTime + clip.length;
            //handle.Source.clip = clip;
            handle.Source.loop = false;
            //handle.Source.Play();
            handle.Source.PlayOneShot(clip);
            if (dt > 0)
            {
                handle.Source.time = dt;
            }
        }
    }


    public void StopSound(uint handleId)
    {
        for (int i = 0; i < _usingSoundList.Count; i++)
        {
            var handle = _usingSoundList[i];
            if (handle.HandleId == handleId)
            {
                handle.Source.Stop();
                handle.Source.clip = null;
                _freeSoundSourceList.Push(handle.Source);
                handle.Source = null;
                handle.PosNode = null;

                handle.RecycleToObjectPool();
                _usingSoundList.RemoveAt(i);
            }
        }
    }

    public void StopAllSound()
    {
        for (int i = 0; i < _usingSoundList.Count; i++)
        {
            var handle = _usingSoundList[i];

            handle.Source.Stop();
            handle.Source.clip = null;
            _freeSoundSourceList.Push(handle.Source);
            handle.Source = null;
            handle.PosNode = null;
            handle.RecycleToObjectPool();
        }
        _usingSoundList.Clear();
    }

    public void PauseAllSound()
    {
        for (int i = 0; i < _usingSoundList.Count; i++)
        {
            var handle = _usingSoundList[i];
            handle.Source.Pause();
        }
    }

    public void UnPauseAllSound()
    {
        for (int i = 0; i < _usingSoundList.Count; i++)
        {
            var handle = _usingSoundList[i];
            handle.Source.UnPause();
        }
    }
    #endregion Sound
    public void LateUpdate()
    {
        var t = Time.realtimeSinceStartup;
        int validCnt = 0;
        for (int i = 0; i < _usingSoundList.Count; i++)
        {
            var handle = _usingSoundList[i];
            if (handle.Clip != null)
            {
                var clip = handle.Clip;
                if (clip == null) 
                {
                    _freeSoundSourceList.Push(handle.Source);
                    handle.Source = null;
                    handle.PosNode = null;
                    handle.RecycleToObjectPool();
                }
                else
                {
                   
                    handle.StartTime = t;
                    PlayHandle(handle, clip, t - handle.StartTime);
                    handle.Clip = null;
                    if (i != validCnt)
                    {
                        _usingSoundList[validCnt] = handle;
                    }
                    ++validCnt;
                }
            }
            else if (!handle.Loop && handle.FinishTime <= t)
            {
                handle.Source.Stop();
                handle.Source.clip = null;
                _freeSoundSourceList.Push(handle.Source);
                handle.PosNode = null;
                handle.Source = null;
                handle.PosNode = null;
                handle.RecycleToObjectPool();
            }
            else
            {
                var sourcePos = handle.Source.transform.position;
                if (handle.Loop && handle.PosNode != null)
                {
                    sourcePos = handle.PosNode.position;
                    sourcePos.z = AudioListenerNode.position.z;
                    handle.Source.transform.position = sourcePos;
                }
                var dist = (sourcePos - AudioListenerNode.position).magnitude;
                handle.Source.volume = _soundVolume * handle.Volume * GetVolume(dist);

                if (i != validCnt)
                {
                    _usingSoundList[validCnt] = handle;
                }
                ++validCnt;
            }
        }
        if (validCnt < _usingSoundList.Count)
        {
            _usingSoundList.RemoveRange(validCnt, _usingSoundList.Count - validCnt);
        }

    }
}