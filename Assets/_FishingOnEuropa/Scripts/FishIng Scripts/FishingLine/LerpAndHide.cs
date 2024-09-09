



//THIS IS AN OLD SCRIPT AND IS JUST HERE FOR REFERANCE WILL BE REMOVED ASAP





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LerpAndHide : MonoBehaviour
//{
//    public Transform targetTransform;
//    public float lerpSpeed = 2.0f;
//    public float hideDelay = 0f;
//    public GameObject fishingLine;

//    private bool isMoving = false;
//    public RodCast rodCastScript;
//    public ClosestObjectsFinder resetfishscript;
    

//    void Start()
//    {
        
//        //StartLerp();
//    }

//    public void StartLerp()
//    {
//        print("LerpEnterd");
//        if (targetTransform != null && isMoving == false)
//        {
            
//            StartCoroutine(LerpToTarget());
//        }
//    }

//    IEnumerator LerpToTarget()
//    {
//        isMoving = true;

//        float journeyLength = Vector3.Distance(transform.position, targetTransform.position);
//        float startTime = Time.time;

//        while (Vector3.Distance(transform.position, targetTransform.position) > 0.7f)
//        {
//            float distanceCovered = (Time.time - startTime) * lerpSpeed;
//            float fractionOfJourney = distanceCovered / journeyLength;

//            transform.position = Vector3.Lerp(transform.position, targetTransform.position, fractionOfJourney);
//            yield return null;
//        }

//        // Hide the object after the lerp is complete
       
//        gameObject.SetActive(false);
//        fishingLine.SetActive(false);
//        resetfishscript.FishReset();

//        isMoving = false;
//        rodCastScript.casted = false;
//        //print("casted = " + rodCastScript.casted);
//    }
//}
