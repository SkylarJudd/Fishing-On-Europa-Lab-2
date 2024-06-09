using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellScript : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SellTriggerEnter>().OnEnterEvent.AddListener(InsideSell);
    }


    public void InsideSell(GameObject go)
    {
        go.SetActive(false);
    }
}
