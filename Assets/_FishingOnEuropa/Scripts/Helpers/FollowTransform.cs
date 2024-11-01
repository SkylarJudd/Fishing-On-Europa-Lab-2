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

    [SerializeField] bool useLate = false;
    [SerializeField] bool useOffset = false;

    void Update()
    {
        if (useLate)
            return;

        if (targetObject == null || destinationObject == null) return;

        Vector3 targetPosition = targetObject.transform.position;

        if (useOffset)
            targetPosition = targetPosition + positionOffset;

        destinationObject.transform.position = targetPosition;
    }

    private void LateUpdate()
    {

        if (!useLate)
            return;

        if (targetObject == null || destinationObject == null) return;


        Vector3 targetPosition = targetObject.transform.position;

        if (useOffset)
            targetPosition = targetPosition + positionOffset;

        destinationObject.transform.position = targetPosition;
    }
}
