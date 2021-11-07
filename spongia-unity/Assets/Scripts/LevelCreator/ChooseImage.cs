using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleFileBrowser;
using System.IO;

public class ChooseImage : MonoBehaviour
{
    public string Path;
    public InputField SpojObazkov;
    public GameObject Background;
    public Image background2;
    public Sprite BackgroundImage;
    public Sprite Pokus1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AssignImage()
    {
        //Pokus1 = Sprite.Create(LoadPNG(@"C:\Users\amiko\Pictures\50zeton.png"),new Rect(0,0,0,0),new Vector2(0.5f,0.5f),100f);
        //Background.GetComponent<RawImage>().texture = LoadPNG(@"C:\Users\amiko\Pictures\50zeton.png");
        //EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //eventSystem.enabled = false;
        FileBrowser.SetFilters( false, new SimpleFileBrowser.FileBrowser.Filter( "Images", ".png" ));
        //SimpleFileBrowser.FileBrowser.SetDefaultFilter( ".png" );
        SimpleFileBrowser.FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
							   () => { Debug.Log( "Canceled" ); },
								   SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, null, "Select Folder", "Select" );
        print(SimpleFileBrowser.FileBrowser.Success);
        //print(SimpleFileBrowser.FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load" )) ;
        StartCoroutine( ShowLoadDialogCoroutine() );
    }
    IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: both, Allow multiple selection: true
		// Initial path: default (Documents), Initial filename: empty
		// Title: "Load File", Submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Files, true, null, null, "Load Files and Folders", "Load" );

		// Dialog is closed
		// Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
		Debug.Log( FileBrowser.Success );

        if (SimpleFileBrowser.FileBrowser.Success)
        {
            string Path = string.Join("",SimpleFileBrowser.FileBrowser.Result);
            //eventSystem.enabled= true;
            Background.GetComponent<RawImage>().texture = LoadPNG(Path);
            SpojObazkov.text=Path;
            
        }
		//string Path = string.Join("",SimpleFileBrowser.FileBrowser.Result);
        //eventSystem.enabled= true;
        //Background.GetComponent<RawImage>().texture = LoadPNG(Path);    
    }
    static Texture2D LoadPNG(string filePath)
     {
 
         Texture2D tex = null;
         byte[] fileData;
 
         if (File.Exists(filePath))
         {
             fileData = File.ReadAllBytes(filePath);
             tex = new Texture2D(2, 2);
             tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
         }
         return tex;
     }
}     
