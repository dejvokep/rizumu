using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class menuSoundManager : MonoBehaviour
{
    public AudioSource clickSound;

    public void playClickSound()
    {
        clickSound.Play();
    }
}
