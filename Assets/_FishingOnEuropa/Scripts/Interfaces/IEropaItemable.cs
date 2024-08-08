using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEropaItemable
{
    
    void OnPickUp();
    void OnDrop();
    void OnPlaceInInventory();
    void OnRemoveFromInventory();

}
