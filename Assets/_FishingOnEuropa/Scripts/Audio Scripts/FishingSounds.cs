using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSounds : MonoBehaviour
{
    [Header("Fishing Rod")]
    public StudioEventEmitter lineBreak;
    public StudioEventEmitter lineThrow;
    public StudioEventEmitter lureDrop;
    public StudioEventEmitter reel;

    [Header("Capture")]
    public StudioEventEmitter hybridCapture;

    public void PlaySound(StudioEventEmitter _name)
    {
        _name.Play();
    }
}
