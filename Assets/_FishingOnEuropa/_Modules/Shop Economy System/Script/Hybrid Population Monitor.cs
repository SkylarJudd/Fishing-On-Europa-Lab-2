using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class monitors the hybrid/fish population
public class HybridPopulationMonitor : MonoBehaviour
{
    [Header("Price Modifiers")]
    public float priceModifier; //charm cost based on hybrid/fish population
    public float greenModifier = 1;
    public float yellowModifier = 0.75f;
    public float redModifier = 0.5f;
    [Header("Population Number")]
    public float currentPopulation = 100;
    public float maxPopulation = 100;
    public float populationPercentage = 1;
    [Header("Population Rates")]
    public float growthRate = -1;
    [Header("Population UI")]
    public GameObject greenIndicator;
    public GameObject yellowIndicator;
    public GameObject redIndicator;
    public GameObject arrowUp;
    public GameObject arrowDown;
    public float minGreen = 0.8f;
    public float minYellow = 0.4f;
    
    private void Start()
    {
        GetPopulation();
    }
    //Updates the new population growth rate
    public void GetPopulationRate(int _growthRate)
    {
        growthRate = _growthRate;
        ReflectPopulationRate();
    }
    //Updates data on current population, growth rate and charm cost
    public void GetPopulation()
    {
        //calculates population as a percentage value
        populationPercentage = currentPopulation / maxPopulation;
        //reflect populationrate on UI
        ReflectPopulationRate();
        //reflects and updates the new population data and charm cost based on population
        greenIndicator.SetActive(false);
        yellowIndicator.SetActive(false);
        redIndicator.SetActive(false);
        if (populationPercentage >= minGreen)
        {
            greenIndicator.SetActive(true);
            priceModifier = greenModifier;
            GetPopulationRate(-1);
        }
        if (populationPercentage >= minYellow && populationPercentage < minGreen)
        {
            yellowIndicator.SetActive(true);
            priceModifier = yellowModifier;
        }
        if (populationPercentage < minYellow)
        {
            redIndicator.SetActive(true);
            priceModifier = redModifier;
        }
    }
    //Reflects whether the growth rate is growing or declining in the UI
    public void ReflectPopulationRate()
    {
        if (growthRate >= 0)
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(false);
        }
        else
        {
            arrowUp.SetActive(false);
            arrowDown.SetActive(true);
        }
    }
    //update population growth/reduction when a day has passed
    public void GetNewPopulation()
    {
        float newPopulation = currentPopulation;
        currentPopulation = newPopulation + growthRate;
        if (currentPopulation > maxPopulation)
            currentPopulation = maxPopulation;
        if (currentPopulation < 0)
            currentPopulation = 0;
        GetPopulation();
    }
    //event subscriptions
    //note: may want use fn getnew population when a day is passed
}
