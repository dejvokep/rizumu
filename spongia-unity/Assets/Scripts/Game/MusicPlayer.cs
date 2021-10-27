using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{

    // Mixer
    public AudioMixerGroup mixer;

    // Tracks by file names
    private Dictionary<string, AudioClip> tracks = new Dictionary<string, AudioClip>();
    // Source
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        // Load all tracks
        Object[] trackArray = Resources.LoadAll("Music", typeof(AudioClip));
        // For each track
        foreach (var track in trackArray) {
            // Cast to clip
            AudioClip clip = (AudioClip) track;
            // Add
            tracks.Add(clip.name, clip);
        }
        // Set source
        source = gameObject.AddComponent<AudioSource>();
        // Set mixer
        source.outputAudioMixerGroup = mixer;

        // Play [DEBUG ONLY]
        Play("tajne_zaznamy");
        Invoke("Pause", 5);
    }

    bool Play(string name) {
        // Non-existing name
        if (!tracks.ContainsKey(name))
            return false;

        // Pause
        Pause();
        // Set clip
        source.clip = tracks[name];
        // Play
        source.Play(0);
        return true;
    }

    void Pause() {
        source.Pause();
        Debug.Log("Current TIME: " + Time());
    }

    float Time() {
        return source.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
