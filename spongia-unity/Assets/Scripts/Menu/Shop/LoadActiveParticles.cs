using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.UI;
public class LoadActiveParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        string activeSkinsPath = Application.persistentDataPath + "/activeSkins.json";
        Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(activeSkinsPath));
        
        int collor1 = Int32.Parse(equipedSkins["particle_skin_color"].Split(',')[0]);
        int collor2 = Int32.Parse(equipedSkins["particle_skin_color"].Split(',')[1]);
        
        foreach (Transform smallchild in transform)
        {
            int[] colorArray = hexToRgb(collor1); 
            Color32 color = new Color32((byte)colorArray[0], (byte)colorArray[1], (byte)colorArray[2],255);
            colorArray = hexToRgb(collor2); 
            Color32 color2 = new Color32((byte)colorArray[0], (byte)colorArray[1], (byte)colorArray[2],255);
            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(color,color2);
            var settings = smallchild.GetComponent<ParticleSystem>().main;
            settings.startColor = gradient;
        }
    }
    
    public int[] hexToRgb(int bigint) {
        
        int r = (bigint >> 16) & 255;
        int g = (bigint >> 8) & 255;
        int b = bigint & 255;

        return new [] {r, g, b};
    }

}
