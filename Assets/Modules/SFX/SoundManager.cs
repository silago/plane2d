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
    public Dictionary<string, Dictionary<string, AudioClip>> _effects = new Dictionary<string, Dictionary<string, AudioClip>>();
    private AudioSource _musicSource;
    private const string DEFAULT_GROUP_NAME = "_";
    private void Awake()
    {
        foreach (var group in effects)
        {
            if (string.IsNullOrEmpty(group.id)) group.id = DEFAULT_GROUP_NAME;
            var effectGroup = _effects[group.id] = new Dictionary<string, AudioClip>();
            foreach (var effect in group.effects)
                effectGroup[effect.id] = effect.clip;
        }
        
        _pool = new Pool<AudioSource>(sourcePrefab);
        _musicSource = _pool.Pick();
        _musicSource.gameObject.name = "music"; 
        _musicSource.loop = false;
        StartCoroutine(StartMusicQueue());
        
        this.Subscribe<PlayOnceMessage>(OnPlayMessage);
        this.Subscribe<PlayLongMessage>(OnLongPlay);
        this.Subscribe<PlayMusicMessage>(OnMusicPlay);
    }

    private AudioClip _nextMusicClip = null;
    private void OnMusicPlay(PlayMusicMessage obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var clip = GetClip(group, soundName);
        if (clip != _musicSource.clip)
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
                _musicSource.PlayOneShot(_nextMusicClip);
                _nextMusicClip = null;
                yield return new WaitForSeconds(_nextMusicClip.length);
            } 
            yield return new WaitForSeconds(5);
        }
    }

    private AudioClip GetClip(string group, string name)
    {
        
        if (!_effects.TryGetValue(group, out var musicGroup)
                || !@musicGroup.TryGetValue(name, out var effect)) return null;
        return effect;
    }
    private void OnLongPlay(PlayLongMessage obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var source = _pool.Pick();
        source.clip = GetClip(group,name);
        source.loop = true;
        source.Play();
        obj.Complete += () => { source.Stop(); _pool.Return(source); };
    }
    private void OnPlayMessage(PlayOnceMessage obj)
    {
        var (group, soundName) = (string.IsNullOrEmpty(obj.Group)) ? SplitSoundName(obj.Id) : (obj.Group, obj.Id);
        var effect  = GetClip(group,soundName);
        var source =  _pool.Pick();
        source.PlayOneShot(effect);
        this.DoWithDelay(effect.length, () =>
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
    public static void PlayEffect(this object @self, string soundName, string group)
    {
        @self.SendEvent(new PlayOnceMessage(group, group));
    }
    
    public static void PlayLong(this object @self, string soundName, string group = "", Action completion = null)
    {
        @self.SendEvent(new PlayLongMessage(group, soundName, completion));
    }
    

}

public class PlayMusicMessage : BaseSoundMessage
{
    public PlayMusicMessage(string group, string id) : base(group, id) {}
    public PlayMusicMessage(string id) : base(id) {}
}

public class PlayLongMessage : BaseSoundMessage
{
    public event Action Complete;
    public PlayLongMessage(string group, string id, Action complete) : base(group, id)
    {
        Complete = complete;
        Group =group;
        Id = id;
    }
    public void Stop()
    {
        Complete?.Invoke();
    }
    public PlayLongMessage(string id, Action complete) : base(id)
    {
        Complete = complete;
        Group = null;
        Id = id;
    }
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

public class PlayOnceMessage : BaseSoundMessage
{
    public Action Callback;
    public PlayOnceMessage(string @group, string id) : base(@group, id) {}
    public PlayOnceMessage(string id) : base(id) {}
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
}