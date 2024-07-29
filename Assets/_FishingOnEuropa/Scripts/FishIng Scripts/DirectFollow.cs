using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectFollow : MonoBehaviour
{

    [SerializeField] Transform followAttachPoint;
    // Update is called once per frame
    void Update()
    {
        transform.position = followAttachPoint.transform.position;
        transform.rotation = followAttachPoint.transform.rotation;
        transform.localScale = followAttachPoint.transform.localScale;
    }
}
