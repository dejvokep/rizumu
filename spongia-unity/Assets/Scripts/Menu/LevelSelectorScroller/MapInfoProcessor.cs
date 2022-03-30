using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoProcessor : MonoBehaviour
{
    private string mapID;

    public GameObject mapImage;
    [Space]
    // public Vector2 imageToScreen = new Vector2(0.655f, 0.73f);
    [Space]
    public Text mapNameSongAuthorLabel;
    public Text mapDifficultyLabel;
    public Text userHighscoreLabel;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mapID != ScrollUnitButton.selectedMapID && ScrollUnitButton.selectedMapID != null)
        {
            mapID = ScrollUnitButton.selectedMapID;

            // mapImage Setup
            mapImage.GetComponent<Image>().sprite = ScrollPopulator.mapsSprites[mapID];
            mapImage.GetComponent<AspectRatioFitter>().aspectRatio = ScrollPopulator.mapsSpritesAspectRatio[mapID];
        }
        else if (ScrollPopulator.mapsID[0] != null && ScrollPopulator.mapsID[0] != "" && mapID == null)
            ScrollUnitButton.selectedMapID = ScrollPopulator.mapsID[0];
    }
}
