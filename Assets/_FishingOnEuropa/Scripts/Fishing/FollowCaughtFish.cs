using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCaughtFish : MonoBehaviour
       
{
    public GameObject fishToTrack;
    bool tracking = false;

    public void TrackFish(GameObject fishToTrackIn)
    {
        fishToTrack = fishToTrackIn;
        tracking = true;

    }

    public void StopTracking()
    {
        tracking = false;
    }

    private void LateUpdate()
    {
        if (tracking == true)
            gameObject.transform.position = fishToTrack.transform.position;
    }
}
