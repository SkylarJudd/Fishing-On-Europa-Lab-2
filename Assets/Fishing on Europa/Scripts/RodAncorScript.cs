using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RodPositionState
{
    Left,
    Middle,
    Right
}
public class RodAncorScript : MonoBehaviour
{
    [SerializeField] GameObject fishingRod;
    [SerializeField] Transform lure;

    

    public RodPositionState rodPositionState;
    public float middleRange = 0.5f;   //used to work out the range that will determin if the rod is in the middle 


    public void UpdateRodAnchor()
    {
        //print("Updating Anchor");
        gameObject.transform.position = fishingRod.transform.position;
        gameObject.transform.LookAt(lure);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
           // print("Space Pressed");
            UpdateRodAnchor();
        }

        Vector3 rodLocalPos = transform.InverseTransformPoint(fishingRod.transform.position);

        //float rodX = fishingRod.transform.position.x;
        //float rodAnchor = transform.position.x;

        if (rodLocalPos.x <  - middleRange)
        {
        rodPositionState = RodPositionState.Left;
        }
        else if (rodLocalPos.x >  + middleRange)
        {
        rodPositionState = RodPositionState.Right;
        }
        else
        {
        rodPositionState = RodPositionState.Middle;
        }

        //print("Current Rod Position " + rodPositionState);

    }
}
