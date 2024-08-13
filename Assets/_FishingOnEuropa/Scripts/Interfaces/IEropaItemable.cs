using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public interface IEropaItemable
{
    
    void OnPickUp(Hand _Hand , Grabbable _Grabbable);
    void OnDrop(Hand _Hand, Grabbable _Grabbable);
    void OnPlaceInInventory();
    void OnRemoveFromInventory();

}
