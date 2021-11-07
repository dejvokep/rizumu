using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.Networking;

public class ChangeAudio : MonoBehaviour

{
    public AudioImporter importer;
    public AudioSource audioSource;
    public AudioSource m_AudioSource;
    public string soundPath;
    public AudioClip audioClip ;
    public UnityWebRequest www;
    public Slider mSlider;
    public AudioClip m_AudioClip;
    public Text MaximumTime;
    public InputField SpojHudba;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChooseAudio()
    {
        //audioClip = audioSource.clip;
        SimpleFileBrowser.FileBrowser.SetDefaultFilter( ".mp3" );
        FileBrowser.SetFilters( false, new FileBrowser.Filter( "Music", ".mp3"));
        SimpleFileBrowser.FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
							   () => { Debug.Log( "Canceled" ); },
								   SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, null, "Select Folder", "Select" );
        print(SimpleFileBrowser.FileBrowser.Success);
        StartCoroutine( ShowLoadDialogCoroutine() );
    }
    IEnumerator ShowLoadDialogCoroutine()
	{
		
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Files, true, null, null, "Load Files and Folders", "Load" );

		
		Debug.Log( FileBrowser.Success );

        if (SimpleFileBrowser.FileBrowser.Success)
        {
            
            string Path = string.Join("",SimpleFileBrowser.FileBrowser.Result);
            print(Path);
            
            importer.Loaded += OnLoaded;
            importer.Import(Path);
            SpojHudba.text=Path;
            //MaximumTime.text = Len.ToString();
            //audioSource = gameObject.AddComponent<AudioSource>();
            //soundPath = Path;
            
            
        }
		 
    }
    private void OnLoaded(AudioClip clip)
    {
        audioSource.clip=clip;
        
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioClip = m_AudioSource.clip;
        Debug.Log("Audio clip length : " + m_AudioSource.clip.length);
        float Len = (float)m_AudioSource.clip.length;
        mSlider.maxValue= Len;
        MaximumTime.text = Len.ToString();
        audioSource.Play();

    }
    IEnumerator LoadAudio(string MyPath)
    {
        print("file://" + MyPath);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///"+MyPath, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
            
            

        
        
    }
    
}
