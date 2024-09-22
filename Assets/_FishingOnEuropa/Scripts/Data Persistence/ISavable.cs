using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    void LoadData(FOEDataThatHasBeenLoaded data);
    void SaveData(ref FOEDataThatHasBeenLoaded data);
}
