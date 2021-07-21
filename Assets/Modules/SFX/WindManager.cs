using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindManager : MonoBehaviour
{
    [SerializeField]
    private string[] names;
    [SerializeField]
    private bool runOnStart = true;

    private void Start()
    {
        if (runOnStart) Run();
    }
    public void Run()
    {
        PlaySound();
    }
    private void PlaySound()
    {
        var soundName = names[Random.Range(0, names.Length)];
        this.SendEvent(new PlaySoundOnce(soundName) {
            Callback = PlaySound
        });
    }
}
