using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxMoonOrbit : MonoBehaviour
{
    [SerializeField] Transform jupiter;
    [SerializeField] float orbitSpeed;
    [SerializeField] float distanceFromJupiter;

    void Update()
    {
        //Vector3 relativePos = (jupiter.position + new Vector3(0, 0, 0)) - transform.position;
        //Quaternion rotation = Quaternion.LookRotation(relativePos);

        //Quaternion current = transform.localRotation;

        //transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * orbitSpeed);
        //transform.Translate(0, 0, 3 * Time.deltaTime);

        float angle = Time.time * orbitSpeed;
        var positionCenterObject = jupiter.position;

        var x = positionCenterObject.x + Mathf.Cos(angle) * distanceFromJupiter;
        var z = positionCenterObject.z + Mathf.Sin(angle) * distanceFromJupiter;
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
