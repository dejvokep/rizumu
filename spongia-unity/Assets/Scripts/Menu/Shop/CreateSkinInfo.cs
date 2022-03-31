using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class CreateSkinInfo : MonoBehaviour
{
    public GameObject ScrollPlayer;
    public GameObject ScrollNote;
    public GameObject ScrollParticle;
    // Start is called before the first frame update
    void Awake()
    {
        string activeSkinsPath = Application.persistentDataPath + "/activeSkins.json";
        string skinInfoPath = Application.persistentDataPath + "/skinInfo.json";
        string json;

        if (!File.Exists(activeSkinsPath))
        {
            Dictionary<string, string> equipedSkinsTemp = new Dictionary<string, string>();
            equipedSkinsTemp["particle_skin_color"] = "16777215,16777215";
            equipedSkinsTemp["player_skins"] = "Default";
            equipedSkinsTemp["particle_skins"] = "Default";
            equipedSkinsTemp["note_skins"] = "Default";
            json = JsonConvert.SerializeObject(equipedSkinsTemp, Formatting.Indented);
            File.WriteAllText(activeSkinsPath, json);
        }

        if (!File.Exists(skinInfoPath))
        {
            string[] skinsList = new string[] {"particle_skins", "player_skins", "note_skins"};
            Dictionary<string, Dictionary<string, bool>> SkinInfo = new Dictionary<string, Dictionary<string, bool>>();

            foreach (string skinType in skinsList)
            {
                if (skinType == "particle_skins")
                {
                    Particles[][] particlesFolder = {Resources.LoadAll<Particles>("Skins/" + skinType)};
                    SkinInfo.Add(skinType, new Dictionary<string, bool>());
                    
                    foreach (Particles[] particle in particlesFolder)
                    {
                        foreach (Particles particleData in particle)
                        {
                            if (particleData.name == "Default")
                            {
                                SkinInfo[skinType][particleData.name] = true;
                            }
                            else
                            {
                                SkinInfo[skinType][particleData.name] = false;
                            }
                            
                        }
                    }

                }
                else
                {
                    Skins[][] skinsFolder = {Resources.LoadAll<Skins>("Skins/" + skinType)};
                    SkinInfo.Add(skinType, new Dictionary<string, bool>());
                    
                    foreach (Skins[] skin in skinsFolder)
                    {
                        foreach (Skins skinData in skin)
                        {
                            if (skinData.name == "Default")
                            {
                                SkinInfo[skinType][skinData.name] = true;
                            }
                            else
                            {
                                SkinInfo[skinType][skinData.name] = false;
                            }
                            
                        }
                    }
                }
            }
            
            json = JsonConvert.SerializeObject(SkinInfo, Formatting.Indented);
            print(json);
            File.WriteAllText(skinInfoPath, json);
            
        }
        ScrollPlayer.GetComponent<PlayerChooser>().ChoosePlayer();
        ScrollNote.GetComponent<NoteChooser>().NotePlayer();
        ScrollParticle.GetComponent<particleChooser>().NotePlayer();
    }
}
