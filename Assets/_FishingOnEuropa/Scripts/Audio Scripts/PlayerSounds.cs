using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Movement")]
    public StudioEventEmitter splash;
    public StudioEventEmitter collision;
    public StudioEventEmitter footsteps;

    [Header("Interactions")]
    public StudioEventEmitter grab;

    public void PlaySound(StudioEventEmitter _name)
    {
        _name.Play();
    }
}
