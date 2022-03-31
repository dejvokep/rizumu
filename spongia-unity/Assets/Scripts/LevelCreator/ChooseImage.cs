using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleFileBrowser;
using System.IO;

public class ChooseImage : MonoBehaviour {
    
    public void AssignImage() {
        // Apply filters
        FileBrowser.SetFilters( false, new SimpleFileBrowser.FileBrowser.Filter( "Images", ".png" ));
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
            // Set texture
            GetComponent<RawImage>().texture = LoadPNG(path);
            // Set path
            AudioLength.imagePath=path;
        }
    }

    private Texture2D LoadPNG(string filePath) {
         // Texture
         Texture2D tex = null;
         byte[] fileData;

         // If exists
         if (File.Exists(filePath)) {
             // Load
             fileData = File.ReadAllBytes(filePath);
             tex = new Texture2D(2, 2);
             tex.LoadImage(fileData);
         }

         return tex;
     }
}     