using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttatch : XRGrabInteractable
{

    public Transform leftAttachedTransform;
    public Transform rightAttachedTransform;

     protected override void OnSelectEntered(SelectEnterEventArgs args)
     {
         //print("Hello?");
         //print(args.interactableObject.transform.tag);

         if ( args.interactableObject.transform.CompareTag("LeftHandTag"))
         {
             //print("Left Tag Enterd");
             attachTransform = leftAttachedTransform;
             gameObject.transform.localScale = new Vector3(1, 1, 1);
         }
         else if (args.interactableObject.transform.CompareTag("RightHandTag"))
         {
             //print("Right Tag Enterd");
             attachTransform = rightAttachedTransform;
             gameObject.transform.localScale = new Vector3(-1, 1, 1);
         }



         base.OnSelectEntered(args);
     } 

     public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        //Debug.Log("GetAttachTransform");

        Transform i_attachTransform = null;

        if (interactor.transform.CompareTag("LeftHandTag"))
        {
            //Debug.Log("Left");
            i_attachTransform = leftAttachedTransform;
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (interactor.transform.CompareTag("RightHandTag"))
        {
            //Debug.Log("Right");
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            i_attachTransform = rightAttachedTransform;
        }
        return i_attachTransform != null ? i_attachTransform : base.GetAttachTransform(interactor);
    }


}
