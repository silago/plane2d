using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Events;
using Modules.Common;
using Modules.Utils;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sourcePrefab;
    private Pool<AudioSource> _pool;
    public EffectGroup[] effects;
    public Dictionary<string, Dictionary<string, AudioEffect>> _effects = new Dictionary<string, Dictionary<string, AudioEffect>>();
    private AudioSource _musicSource;
    private const string DEFAULT_GROUP_NAME = "_";
    private void Awake()
    {
        foreach (var group in effects)
        {
            if (string.IsNullOrEmpty(group.id)) group.id = DEFAULT_GROUP_NAME;
            var effectGroup = _effects[group.id] = new Dictionary<string, AudioEffect>();
            foreach (var effect in group.effects)
                effectGroup[effect.id] = effect;
        }
        
        _pool = new Pool<AudioSource>(sourcePrefab);
        _musicSource = _pool.Pick();
        _musicSource.gameObject.name = "music"; 
        _musicSource.loop = false;
        StartCoroutine(StartMusicQueue());
        
        this.Subscribe<PlaySoundOnce>(OnPlayMessage);
        this.Subscribe<PlaySoundLong>(OnLongPlay);
        this.Subscribe<PlayMusicMessage>(OnMusicPlay);
    }

    private AudioEffect _nextMusicClip = null;
    private void OnMusicPlay(PlayMusicMessage obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var clip = GetClip(group, soundName);
        if (clip.clip != _musicSource.clip)
        {
            _nextMusicClip = clip; 
        }
    }

    private Coroutine _musicQueue;
    IEnumerator StartMusicQueue()
    {
        while (true)
        {
            if (_musicSource.isPlaying == false && _nextMusicClip != null)
            {
                _musicSource.volume = _nextMusicClip.volume;
                _musicSource.PlayOneShot(_nextMusicClip.clip);
                yield return new WaitForSeconds(_nextMusicClip.clip.length);
                _nextMusicClip = null;
            } 
            yield return new WaitForSeconds(5);
        }
    }

    private AudioEffect GetClip(string group, string name)
    {
        
        if (!_effects.TryGetValue(group, out var musicGroup)
                || !@musicGroup.TryGetValue(name, out var effect)) return null;
        return effect;
    }
    private void OnLongPlay(PlaySoundLong obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var source = _pool.Pick();
        var effect = GetClip(group,soundName);
        source.volume = effect.volume;
        source.clip = effect.clip;
        source.loop = true;
        source.Play();
        obj.Complete += () => { source.Stop(); _pool.Return(source); };
    }
    private void OnPlayMessage(PlaySoundOnce obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var effect  = GetClip(group,soundName);
        var source =  _pool.Pick();
        source.volume = effect.volume;
        source.PlayOneShot(effect.clip);
        this.DoWithDelay(effect.clip.length, () =>
        {
            _pool.Return(source);
            obj.Callback?.Invoke();
        });
    }
    
    public static (string,string) SplitSoundName(string name)
    {
        string effect = null;
        string group = null;
        var parts = name.Split('.');
        if (parts.Length == 1)
        {
            effect = parts[0];
        }
        else if (parts.Length == 2)
        {
            group = parts[0];
            effect = parts[1];
        }
        if (group == null) group = DEFAULT_GROUP_NAME;
        return (group, effect);
    }
}

public static class SoundObjectExtensions
{
    public static void PlayMusic(this object @self, string soundName, string group = "")
    {
        @self.SendEvent(new PlayMusicMessage(group, soundName));
    }
    public static void PlayEffect(this object @self, string soundName, string group = null)
    {
        @self.SendEvent(new PlaySoundOnce( soundName));
    }
    
    public static void PlayLong(this object @self, string soundName, string group = "", Action completion = null)
    {
        @self.SendEvent(new PlaySoundLong(group, soundName));
    }
    

}

public class PlayMusicMessage : BaseSoundMessage
{
    public PlayMusicMessage(string group, string id) : base(group, id) {}
    public PlayMusicMessage(string id) : base(id) {}
}

public class PlaySoundLong : BaseSoundMessage
{
    public event Action Complete;
    public PlaySoundLong(string group, string id) : base(group, id) {}
    public void Stop()
    {
        Complete?.Invoke();
    }
    public PlaySoundLong(string id) : base(id) {}
}

public abstract class BaseSoundMessage : IMessage
{
    public string Group ;
    public string Id;
    public BaseSoundMessage(string group, string id)
    {
        Group =group;
        Id = id;
    }
    public BaseSoundMessage(string id)
    {
        Group = null;
        Id = id;
    }
}

public class PlaySoundOnce : BaseSoundMessage
{
    public Action Callback;
    public PlaySoundOnce(string @group, string id) : base(@group, id) {}
    public PlaySoundOnce(string id) : base(id) {}
}

[Serializable]
public class EffectGroup
{
    public string id;
    public AudioEffect[] effects;

}

[Serializable]
public class AudioEffect
{
    public string id;
    public AudioClip clip;
    [Range(0,1)]
    public float volume = 1;
}