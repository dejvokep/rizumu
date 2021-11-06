using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class SFXPlayer : MonoBehaviour
{

    public enum EffectType {
        CLICK, CLICK_BACK, CLICK_CONTINUE, CLICK_RETRY, DRUM_HIT, HOVER
    }

    // Effects
    private static Dictionary<EffectType, AudioSource> effects = new Dictionary<EffectType, AudioSource>();
    // Mixer
    public AudioMixerGroup mixer;

    // Audio clips
    public AudioClip click, clickBack, clickContinue, clickRetry, drumHit, hover;

    // Start is called before the first frame update
    void Start()
    {
        // For each
        foreach (EffectType type in Enum.GetValues(typeof(EffectType))) {
            // Add audio source
            AudioSource source = gameObject.AddComponent<AudioSource>();
            // Set mixer
            source.outputAudioMixerGroup = mixer;

            // Set clip
            if (type == EffectType.CLICK)
                source.clip = click;
            else if (type == EffectType.CLICK_BACK)
                source.clip = clickBack;
            else if (type == EffectType.CLICK_CONTINUE)
                source.clip = clickContinue;
            else if (type == EffectType.CLICK_RETRY)
                source.clip = clickRetry;
            else if (type == EffectType.DRUM_HIT)
                source.clip = drumHit;
            else
                source.clip = hover;

            // Add
            effects.Add(type, source);
        }
    }

    void OnDestroy() {
        effects.Clear();
    }

    public void Click() {
        Play(EffectType.CLICK);
    }

    public void ClickBack() {
        Play(EffectType.CLICK_BACK);
    }

    public void ClickContinue() {
        Play(EffectType.CLICK_CONTINUE);
    }

    public void ClickRetry() {
        Play(EffectType.CLICK_RETRY);
    }

    public void DrumHit() {
        Play(EffectType.DRUM_HIT);
    }

    public void Hover() {
        Play(EffectType.HOVER);
    }

    public static void Play(EffectType type) {
        effects[type].Play();
    }

}
