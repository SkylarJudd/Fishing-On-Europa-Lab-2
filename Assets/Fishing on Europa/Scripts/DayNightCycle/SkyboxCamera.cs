using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

<<<<<<< HEAD:Assets/Fishing on Europa/Scripts/DayNightCycle/SkyboxCamera.cs
public class SkyboxCamera : MonoBehaviour
{
    [SerializeField] private Transform playerCam;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerCam.rotation;
    }
=======
public interface IEropaItemable
{
    
    void OnPickUp(Hand _Hand , Grabbable _Grabbable);
    void OnDrop(Hand _Hand, Grabbable _Grabbable);
    void OnPlaceInInventory();
    void OnRemoveFromInventory();

>>>>>>> main:Assets/_FishingOnEuropa/Scripts/Interfaces/IEropaItemable.cs
}
