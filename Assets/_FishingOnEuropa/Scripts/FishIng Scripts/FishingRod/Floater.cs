using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour

{
    public Rigidbody floterRB;
    public float depthBeforeSubmerged = 1f;
    public float displacmentAmount = 3f;

    private void FixedUpdate()
    {
        float wavehight = WaveMannagerTheSecond.instance.GetWaveHight(transform.position.x);
        if( transform.position.y < wavehight)
        {
            float displacmentMultiplier = Mathf.Clamp01((wavehight - transform.position.y) / depthBeforeSubmerged) * displacmentAmount;
            floterRB.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacmentMultiplier, 0f), ForceMode.Acceleration);
        }
    }
}
