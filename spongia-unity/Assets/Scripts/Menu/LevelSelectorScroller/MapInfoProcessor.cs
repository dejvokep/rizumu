using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoProcessor : MonoBehaviour
{
    public GameObject mapImage;

    private bool isSelected = false;

    private bool wasScrolling = false;

    void Update()
    {
        if (!isSelected)
        {
            if (ScrollUnitButton.selectedMapID != null)
            {
                string mapID = ScrollUnitButton.selectedMapID;

                mapImage.GetComponent<Image>().sprite = ScrollPopulator.mapsSprites[mapID];
                mapImage.GetComponent<AspectRatioFitter>().aspectRatio = ScrollPopulator.mapsSpritesAspectRatio[mapID];

                isSelected = true;
            }
            return;
        }
    }

    public async void fadeIn()
    {
        string mapID = ScrollUnitButton.selectedMapID;
        
        mapImage.GetComponent<Animator>().SetBool("isScrolling", false);

        await Task.Delay(500);

        try
        {
            mapImage.GetComponent<Image>().sprite = ScrollPopulator.mapsSprites[mapID];
            mapImage.GetComponent<AspectRatioFitter>().aspectRatio = ScrollPopulator.mapsSpritesAspectRatio[mapID];
        }
        catch (MissingReferenceException)
        {
            
        }
    }

    public void fadeOut()
    {
        mapImage.GetComponent<Animator>().SetBool("isScrolling", true);
    }
}
