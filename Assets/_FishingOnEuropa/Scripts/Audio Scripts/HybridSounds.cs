using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridSounds : MonoBehaviour
{
    [Header("Hybrid Interactions")]
    public StudioEventEmitter hybridAngry;
    public StudioEventEmitter hybridHappy;
    public StudioEventEmitter hybridPlay;
    public StudioEventEmitter hybridSad;

    [Header("Hybrid Charm")]
    public StudioEventEmitter hybridSpawnCharm;
    public StudioEventEmitter hybridDropCharm;
    public StudioEventEmitter hybridCharmPickup;

    [Header("Hybrid Eat")]
    public StudioEventEmitter hybridActiveMunch;
    public StudioEventEmitter hybridChomp;
    public StudioEventEmitter hybridSlowMunch;

    [Header("Hybrid Movement")]
    public StudioEventEmitter hybridSplash;
    public StudioEventEmitter hybridDive;
    public StudioEventEmitter hybridSwim;
    public StudioEventEmitter hybridIdle;
    public StudioEventEmitter hybridCollision;

    public void PlaySound(StudioEventEmitter _name)
    {
        _name.Play();
    }
}
