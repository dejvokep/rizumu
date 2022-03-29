using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PulsingObject : MonoBehaviour
{

    AudioSource _audioSource;
    AudioSource[] _audioSources;
    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];

    // Start is called before the first frame update
    void Start()
    {
        _audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        _audioSource = _audioSources[0].isPlaying ? _audioSources[0] : _audioSources[1];
        
        GetSpectrumData();
        MakeFrequencyBands();
    }

    void GetSpectrumData()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }
    }
}