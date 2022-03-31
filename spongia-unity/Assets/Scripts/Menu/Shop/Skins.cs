using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin")]
public class Skins : ScriptableObject
{
    public string name;
    public int cost;
    public bool bought;
    public Sprite skin;
}
