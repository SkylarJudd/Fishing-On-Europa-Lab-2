using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Autohand.Demo{
    public class OpenXRTeleporterLink : MonoBehaviour{
        public Teleporter hand;
        public InputActionProperty startTeleportAction;
        public InputActionProperty finishTeleportAction;
        
        bool teleporting = false;

        void OnEnable() {
            if(startTeleportAction.action != null) startTeleportAction.action.Enable();
            if (startTeleportAction.action != null) startTeleportAction.action.performed += StartTeleportAction;
            if (finishTeleportAction.action != null) finishTeleportAction.action.Enable();
            if (finishTeleportAction.action != null) finishTeleportAction.action.performed += FinishTeleportAction;
        }

        void OnDisable() { 
            if (startTeleportAction.action != null) startTeleportAction.action.performed -= StartTeleportAction;
            if (finishTeleportAction.action != null) finishTeleportAction.action.performed -= FinishTeleportAction;
        }


        void StartTeleportAction(InputAction.CallbackContext a)
        {
            var axis = a.ReadValue<Vector2>();
            print(axis);
            if (!teleporting && axis.y > 0.5)
            {
                hand.StartTeleport();
                teleporting = true;
            }
            else if (teleporting && axis.y < 0.3)
            {
                hand.Teleport();
                teleporting = false; 
            }
        }

        void FinishTeleportAction(InputAction.CallbackContext a) {
            if(teleporting){
                hand.Teleport();
                teleporting = false;
            }
        }
    }
}
