using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    //Infomation on what the crop will grow into
    Seeds seedToGrow;

    [Header("Growth Stages")]
    public GameObject seed;
    public GameObject sprout;
    public GameObject adolecent;
    public GameObject mature;
    public enum CropState
    {
        Seed, Sprout, Adolecent, Mature
    }

    //The current stage in the crop's growth
    public CropState cropState;

    //Initialisation for the crop GameObject
    //Called when the player plants a seed
    public void Plant(Seeds seedToGrow)
    {
        //Save the seed information
        this.seedToGrow = seedToGrow;

        //Set the sprout and harvestable GameObjects
        sprout = Instantiate(seedToGrow.sprout, transform);

        //Set the growth stages and harvestable GameObjects
        adolecent = Instantiate(seedToGrow.adolecent, transform);

        //Access the crop item data
        Food foodProduced = seedToGrow.foodProduced;

        //Instantiate the harvestable crop
        mature = Instantiate(foodProduced.gameModel, transform);

        //Set the inital state to Seed
        SwitchState(CropState.Seed);
    }

    public void Grow()
    {

    }

    //function to handle the state change
    void SwitchState(CropState stateToSwitch)
    {
        //Reset everything and set all GameObjects to inactive
        seed.SetActive(false);
        sprout.SetActive(false);
        adolecent.SetActive(false);
        mature.SetActive(false);

        switch (stateToSwitch)
        {
            case CropState.Seed:
                //Enable the Seed GameObject
                seed.SetActive(true);
                break;
            case CropState.Sprout:
                //Enable the Sprout GameObject
                sprout.SetActive(true);
                break;
            case CropState.Adolecent:
                //Enable the Adolecent GameObject
                adolecent.SetActive(true);
                break;
            case CropState.Mature:
                //Enable the MAture GameObject
                mature.SetActive(true);
                break;
        }

        //Set the current crop state to the state we're switching to
        cropState = stateToSwitch;
    }
}
