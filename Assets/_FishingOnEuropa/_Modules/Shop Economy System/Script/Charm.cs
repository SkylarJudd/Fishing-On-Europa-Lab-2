using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
//Scriptable object for storing information for charms
public class Charm : ScriptableObject
{
    public string charmName;
    public Sprite charmImage;
    public int price;
    public int sellPrice;
    [TextArea(8, 10)] public string itemDescription;

}
