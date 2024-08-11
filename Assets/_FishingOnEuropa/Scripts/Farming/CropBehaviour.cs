using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    //Infomation on what the crop will grow into
    public SeedsSO seedToGrow;


    public HarvestBox harvestBox;

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
        Seed, Sprout, Adolecent, Mature, Harvested
    }

    //The current stage in the crop's growth
    public CropState cropState;

    //Initialisation for the crop GameObject
    //Called when the player plants a seed
    public void Plant()
    {
        //Access the crop item data
        FOEItem foodProduced = seedToGrow.foodProduced;

        //Convert Growth Time into minutes (TODO with day/night cycle)
        maxGrowth = seedToGrow.growthTime;

        harvestBox = FindObjectOfType<HarvestBox>();

        //Set the inital state to Seed
        //SwitchState(CropState.Seed);
    }

    //crop will grow when condition is met
    public void Grow()
    {
        //increase the growth point by 1
        growth++;

        //the seed will sprout when the growth is at 33%
        if (growth >= maxGrowth / 4 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Sprout);
        }

        //the sprout will reach adolecent at 63%
        if (growth >= maxGrowth / 4 * 2 && cropState == CropState.Sprout)
        {
            SwitchState(CropState.Adolecent);
        }

        //Fully grown
        if (growth >= maxGrowth / 4 * 3 && cropState == CropState.Adolecent)
        {
            SwitchState(CropState.Mature);
        }

        //Harvested
        if (growth >= maxGrowth && cropState == CropState.Mature)
        {
            SwitchState(CropState.Harvested);
        }

        //Harvested debug
        if (growth >= maxGrowth + 1)
        {
            SwitchState(CropState.Harvested);
        }
    }

    //function to handle the state change
    void SwitchState(CropState stateToSwitch)
    {
        switch (stateToSwitch)
        {
            case CropState.Sprout:
                //Instantiate the sprout game object as child of the seed.
                sprout = Instantiate(seedToGrow.sprout, transform);
                break;

            case CropState.Adolecent:
                //Deactivate Sprout GameObject
                sprout.SetActive(false);

                //Instantiate the adolecent game object as child of the seed.
                adolecent = Instantiate(seedToGrow.adolecent, transform);
                break;

            case CropState.Mature:
                //Deactivate Adolecent GameObject
                adolecent.SetActive(false);

                //Instantiate the Mature game object as child of the seed.
                mature = Instantiate(seedToGrow.mature, transform);
                break;

            case CropState.Harvested:
                //Access the crop item data
                //Food foodProduced = seedToGrow.foodProduced;
                //Instantiate the harvestable crop
                //mature = Instantiate(foodProduced.gameModel, transform);

                //harvestBox.UpdateHarvestValue(seedToGrow.foodProduced.foodItem, 1);

                //unparent to soil
                //mature.transform.parent = null;
                //mature.transform.localScale = new Vector3(1, 1, 1);
                Destroy(gameObject);


                break;
        }

        //Set the current crop state to the state we're switching to
        cropState = stateToSwitch;
    }
}
