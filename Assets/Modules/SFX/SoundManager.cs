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
        _musicSource.loop = false;
        StartCoroutine(StartMusicQueue());
        
        this.Subscribe<PlayOnceMessage>(OnPlayMessage);
        this.Subscribe<PlayLongMessage>(OnLongPlay);
        this.Subscribe<PlayMusicMessage>(OnMusicPlay);
    }

    private AudioClip _nextMusicClip = null;
    private void OnMusicPlay(PlayMusicMessage obj)
    {
        var clip = GetClip(obj.Group, obj.Id);
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
            } 
            yield return new WaitForSeconds(5);
        }
    }

    private AudioClip GetClip(string group, string name)
    {
        if (string.IsNullOrEmpty(group)) group = DEFAULT_GROUP_NAME;
        if (!_effects.TryGetValue(group, out var musicGroup)
                || !@musicGroup.TryGetValue(name, out var effect)) return null;
        return effect;
    }
    private void OnLongPlay(PlayLongMessage obj)
    {
        var source = _pool.Pick();
        source.clip = GetClip(obj.Group,obj.Id);
        source.loop = true;
        source.Play();
        obj.Complete += () => { source.Stop(); _pool.Return(source); };
    }
    private void OnPlayMessage(PlayOnceMessage obj)
    {
        var effect  = GetClip(obj.Group,obj.Id);
        var source = _pool.Pick();
        source.PlayOneShot(effect);
        this.DoWithDelay(effect.length, () =>
        {
            _pool.Return(source);
        });
    }
}

public static class SoundObjectExtensions
{
    public static void PlayMusic(this object @self, string soundName)
    {
        var (group, name) = SplitSoundName(soundName);
        @self.SendEvent(new PlayMusicMessage(group, name));
    }
    public static void PlayEffect(this object @self, string soundName)
    {
        var (group, name) = SplitSoundName(soundName);
        @self.SendEvent(new PlayOnceMessage(group, name));
    }
    
    public static void PlayLong(this object @self, string soundName, Action completion)
    {
        var (group, name) = SplitSoundName(soundName);
        @self.SendEvent(new PlayLongMessage(group, name, completion));
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
        return (group, effect);
    }
}

public class PlayMusicMessage : IMessage
{
    public string Group ;
    public string Id;
    public PlayMusicMessage(string group, string id)
    {
        Group =group;
        Id = id;
    }
}

public class PlayLongMessage : IMessage
{
    public event Action Complete;
    public string Group ;
    public string Id;
    public PlayLongMessage(string group, string id, Action complete)
    {
        Complete = complete;
        Group =group;
        Id = id;
    }
    public PlayLongMessage(string id, Action complete)
    {
        Complete = complete;
        Group = null;
        Id = id;
    }
}

public class PlayOnceMessage : IMessage
{
    public string Group ;
    public string Id;
    public PlayOnceMessage(string group, string id)
    {
        Group =group;
        Id = id;
    }
    public PlayOnceMessage(string id)
    {
        Group = null;
        Id = id;
    }
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