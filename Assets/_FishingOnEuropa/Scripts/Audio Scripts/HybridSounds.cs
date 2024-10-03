using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridSounds : MonoBehaviour
{
    [Header("Hybrid Interactions")]
    public StudioEventEmitter angry;
    public StudioEventEmitter happy;
    public StudioEventEmitter play;
    public StudioEventEmitter sad;

    [Header("Hybrid Charm")]
    public StudioEventEmitter spawnCharm;
    public StudioEventEmitter dropCharm;
    public StudioEventEmitter charmPickup;

    [Header("Hybrid Eat")]
    public StudioEventEmitter activeMunch;
    public StudioEventEmitter chomp;
    public StudioEventEmitter slowMunch;

    [Header("Hybrid Movement")]
    public StudioEventEmitter splash;
    public StudioEventEmitter dive;
    public StudioEventEmitter swim;
    public StudioEventEmitter iIdle;
    public StudioEventEmitter collision;

    public void PlaySound(StudioEventEmitter _name)
    {
        _name.Play();
    }
}
