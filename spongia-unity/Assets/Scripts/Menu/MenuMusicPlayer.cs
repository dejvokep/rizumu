using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        string[] maps = ScrollPopulator.LoadMapsFromResources();
        List<string> availableMaps = new List<string>();
        foreach (string map in maps)
        {
            if (map != null && map != "")
                availableMaps.Add(map);
        }

        int index = Random.Range(0, availableMaps.Count);

        audioSource.clip = Resources.Load<AudioClip>("maps/" + availableMaps[index] + "/audio");
        audioSource.Play();
    }
}
