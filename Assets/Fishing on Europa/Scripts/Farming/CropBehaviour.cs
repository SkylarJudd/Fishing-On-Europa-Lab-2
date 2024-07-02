using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    //Infomation on what the crop will grow into
    public Seeds seedToGrow;

    [Header("Growth Stages")]
    public  GameObject seed;
    private GameObject sprout;
    private GameObject adolecent;
    private GameObject mature;

    //growth points of the crop
    public float growth;
    //how many grouth points it takes before it becomes harvestable
    public float maxGrowth;

    public enum CropState
    {
        Seed, Sprout, Adolecent, Mature
    }

    //The current stage in the crop's growth
    public CropState cropState;

    //Initialisation for the crop GameObject
    //Called when the player plants a seed
    public void Plant()
    {
        //Set the sprout and harvestable GameObjects
        sprout = Instantiate(seedToGrow.sprout, transform);

        //Set the growth stages and harvestable GameObjects
        adolecent = Instantiate(seedToGrow.adolecent, transform);

        //Access the crop item data
        Food foodProduced = seedToGrow.foodProduced;

        //Instantiate the harvestable crop
        mature = Instantiate(foodProduced.gameModel, transform);

        //Convert Growth Time into minutes (TODO with day/night cycle)
        maxGrowth = seedToGrow.growthTime;

        //Set the inital state to Seed
        SwitchState(CropState.Seed);
    }

    //crop will griw when condition is met
    public void Grow()
    {
        //increase the growth point by 1
        growth++;

        //the seed will sprout when the groth is at 33%
        if (growth >= maxGrowth / 3 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Sprout);
        }

        //the sprout will reach adolecent at 63%
        if (growth >= maxGrowth / 3 * 2 && cropState == CropState.Sprout)
        {
            SwitchState(CropState.Adolecent);
        }

        //Fully grown
        if (growth >= maxGrowth && cropState == CropState.Adolecent)
        {
            SwitchState(CropState.Mature);
        }
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
                sprout = Instantiate(seedToGrow.sprout, transform);
                //sprout.SetActive(true);
                break;
            case CropState.Adolecent:
                //Enable the Adolecent GameObject
                adolecent.SetActive(true);
                break;
            case CropState.Mature:
                //Enable the Mature GameObject
                mature.SetActive(true);
                //unparent to soil
                mature.transform.parent = null;
                //
                Destroy(gameObject);
                break;
        }

        //Set the current crop state to the state we're switching to
        cropState = stateToSwitch;
    }
}
