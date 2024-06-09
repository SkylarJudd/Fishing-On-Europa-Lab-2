using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SellTriggerEnter : MonoBehaviour
{
    public string tragetTag;
    public UnityEvent<GameObject> OnEnterEvent;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == tragetTag)
        {
            OnEnterEvent.Invoke(other.gameObject);
        }

    }
}
