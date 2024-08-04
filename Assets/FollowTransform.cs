using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject;

    [Header("Object to Copy Transform To")]
    public GameObject destinationObject;

    [Header("Offsets")]
    public Vector3 positionOffset;

    void Update()
    {
        if (targetObject == null || destinationObject == null) return;

        Vector3 targetPosition = targetObject.transform.position;
        destinationObject.transform.position = targetPosition;
    }
}
