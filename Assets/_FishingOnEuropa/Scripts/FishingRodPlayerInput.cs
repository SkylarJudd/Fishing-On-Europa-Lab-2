using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class FishingRodPlayerInput : MonoBehaviour
{
    [SerializeField] private BoolReference playerCastInput;
    [SerializeField] private BoolReference fishingLineCasted;
    public void OnTriggerPressed(InputAction _context)
    {
        float input = _context.ReadValue<float>();

        if (input == 0 && playerCastInput.Value == true)
        {
            playerCastInput.Value = false;
        }
        else if (input > 0 && playerCastInput.Value == false)
        {
            playerCastInput.Value = true;
        }
        else if (input == 0)
        {
            playerCastInput.Value = false;
        }

    }
}

