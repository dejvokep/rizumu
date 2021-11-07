using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleFileBrowser;
using System.IO;

public class ImageLoader : MonoBehaviour
{
    public string Path;
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
        Background.GetComponent<RawImage>().texture = LoadPNG(@"C:\Users\amiko\Pictures\50zeton.png");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.enabled = false;
        SimpleFileBrowser.FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
							   () => { Debug.Log( "Canceled" ); },
								   SimpleFileBrowser.FileBrowser.PickMode.FilesAndFolders, false, null, null, "Select Folder", "Select" );
        if (SimpleFileBrowser.FileBrowser.Success)
        {
            string Path = string.Join("",SimpleFileBrowser.FileBrowser.Result);
            eventSystem.enabled= true;
            print("EventSystem");
            Background.GetComponent<RawImage>().texture = LoadPNG(Path);
            
            //Pokus1 = Sprite.Create(LoadPNG(Path),new Rect(0,0,0,0),new Vector2(0.5f,0.5f),100f);
            
            //Background.sprite = Pokus1;
        }
		    
    }
    public static Texture2D LoadPNG(string filePath)
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
