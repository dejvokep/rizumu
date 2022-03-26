using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    public AudioSource[] audioSources;

    private List<AudioClip> audioClips;

    private double nextStartTime;
    private float loopPointSeconds;
    private int nextSource;


    void Start()
    {
        string[] maps = ScrollPopulator.LoadMapsFromResources();
        audioClips = new List<AudioClip>();
        foreach (string map in maps)
        {
            if (map != null && map != "")
                audioClips.Add(Resources.Load<AudioClip>("maps/" + map + "/audio"));
        }

        nextStartTime = AudioSettings.dspTime + 1;
        nextSource = 1;

        audioSources[0].Play();
    }

    void Update()
    {
        if (AudioSettings.dspTime > nextStartTime - 1)
        {
            AudioClip clipToPlay = getRandomClip();

            audioSources[nextSource].clip = clipToPlay;
            audioSources[nextSource].PlayScheduled(nextStartTime);

            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            nextStartTime += duration;

            nextSource = 1 - nextSource;
        }
    }

    private AudioClip getRandomClip()
    {
        int index = Random.Range(0, audioClips.Count);
        AudioClip audioClip = audioClips[index];

        return audioClip;
    }
}