using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoProcessor : MonoBehaviour
{
    private string mapID;

    public GameObject mapImage;
    [Space]
    public Vector2 imageToScreen = new Vector2(0.655f, 0.73f);
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

            float viewAspectRation = (imageToScreen.x*Screen.currentResolution.width)/(imageToScreen.y*Screen.currentResolution.height);
            Debug.Log($"{Screen.currentResolution.width} {Screen.currentResolution.height}");
            if (ScrollPopulator.mapsSpritesAspectRatio[mapID] >= viewAspectRation)
            {
                mapImage.GetComponent<AspectRatioFitter>().aspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode.HeightControlsWidth;
                mapImage.GetComponent<RectTransform>().sizeDelta = new Vector2(0, imageToScreen.y * Screen.currentResolution.height);
            }
            else
            {
                mapImage.GetComponent<AspectRatioFitter>().aspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode.WidthControlsHeight;
                mapImage.GetComponent<RectTransform>().sizeDelta = new Vector2(imageToScreen.x * Screen.currentResolution.width, 0);
            }

            // Populate Labels
            mapNameSongAuthorLabel.text = $"{ScrollPopulator.mapsInfo[mapID].song_name} - {ScrollPopulator.mapsInfo[mapID].song_author}";

            mapDifficultyLabel.text = $"Difficulty: {ScrollPopulator.mapsInfo[mapID].difficulty}★";

            if (UserDataReader.userData != null)
                userHighscoreLabel.text = $"Highscore: {UserDataReader.userData.highscores[mapID].score}";
            else
                userHighscoreLabel.text = "Highscore: 0";
        }
        else if (ScrollPopulator.mapsID[0] != null && ScrollPopulator.mapsID[0] != "" && mapID == null)
            ScrollUnitButton.selectedMapID = ScrollPopulator.mapsID[0];
    }
}
