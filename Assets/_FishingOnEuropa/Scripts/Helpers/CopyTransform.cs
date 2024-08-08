using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject;

    [Header("Object to Copy Transform To")]
    public GameObject destinationObject;

    [Header("Offsets")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;

    void Update()
    {
        if (targetObject == null || destinationObject == null) return;

        // Copy Position with Offset
        Vector3 newPosition = targetObject.transform.position + positionOffset;
        destinationObject.transform.position = newPosition;

        Vector3 targetRotation = targetObject.transform.eulerAngles;
        Vector3 newRotation = new Vector3(targetRotation.z + rotationOffset.x, targetRotation.y + rotationOffset.y, targetRotation.x + rotationOffset.z);
        destinationObject.transform.rotation = Quaternion.Euler(newRotation);

        // Copy Scale with Offset
        Vector3 newScale = targetObject.transform.localScale + scaleOffset;
        destinationObject.transform.localScale = newScale;
    }
}
