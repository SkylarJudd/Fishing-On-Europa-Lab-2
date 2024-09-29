using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferanceSingletonExsample : GameBehaviour
{

    private void Start()
    {
        _ES.ExsampleFuctionCall();
        _ES.exsampleParamator = true;
    }

}
