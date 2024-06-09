using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChestZone { Zone1, Zone2, Zone3, NoChest};
public enum FoodType { Veg, Fruit, Meat, Seed, Leaves, Bugs, Flowers};
public enum FoodList { Bugs , WhiteMeat, RedMeat , Leaves , Flowers , Seeds , Carrot , Potato , Oranges , Apples , Bannan , WaterMelon}
[CreateAssetMenu(fileName = "Food", menuName = "Europa/Food", order = 1)]


public class Food : ScriptableObject
{
    
    public ChestZone chestZone;
    public FoodType foodType;
    public FoodList foodName;
    public string foodDesc;
    public bool foundInChest;
    public int growingCropDropSeeds;
    public int finshiedCropDropSeeds;
    public int finshiedCropDropFood;
    public int happinesAdd;


}
