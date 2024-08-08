using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallingSOExsample : MonoBehaviour
{
    public ExsampleSO exsampleSO;

    private void Start()
    {
        print(exsampleSO.ExsampleSting);
        print(exsampleSO.ExsampleInt);
    }
}
