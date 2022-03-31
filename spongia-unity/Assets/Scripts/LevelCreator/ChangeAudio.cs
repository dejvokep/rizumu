using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.Networking;

public class ChangeAudio : MonoBehaviour {

    // Importer
    public AudioImporter importer;
    // Audio source
    public AudioSource audioSource;
    
    // Progress slider
    public Slider progressSlider;
    // Maximum time text
    public Text maximumTimeText;
    
    public void ChooseAudio() {
        // Apply filters
        SimpleFileBrowser.FileBrowser.SetDefaultFilter(".mp3");
        FileBrowser.SetFilters( false, new FileBrowser.Filter("Music", ".mp3"));
        // Show dialog
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((paths) => {}, () => {}, SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, null, "Select Folder", "Select" );
        // Wait
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine() {
        // Wait
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Files, true, null, null, "Load Files and Folders", "Load" );

        // If succeeded
        if (SimpleFileBrowser.FileBrowser.Success) {
            // Path
            string path = string.Join("",SimpleFileBrowser.FileBrowser.Result);
            // Load
            importer.Loaded += OnLoaded;
            importer.Import(path);
            // Set path
            AudioLength.audioPath=path;
        }
		 
    }
    private void OnLoaded(AudioClip clip)
    {
        // Set clip
        audioSource.clip=clip;

        // Reset lengths
        progressSlider.maxValue= (float) clip.length;
        maximumTimeText.text = clip.length.ToString();
    }
    
}