using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.PlayerLoop;

namespace Europa
{
    public class SkyboxCamera : GameBehaviour
    {
        [SerializeField] private Transform skyboxCamTransform;
        [SerializeField] private Transform trackedDriverTransform;




        void Update()
        {

            // transform.rotation = skyboxCamTransform.rotation * trackedDriverTransform.rotation;
            transform.localRotation = _PLAYER.TrackedOffset.localRotation * trackedDriverTransform.localRotation;
        }

    }

}
