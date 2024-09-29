using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    public class FOEItem_Charms : FOEItem, ISellable
    {
        public CharmType charmType;
        public ItemRarity itemRarity;

        [Header("Economy")]
        [SerializeField] private IntVariable basePrice;         //The base price of a charm
        [SerializeField] private float populationPriceVariable; //The price/population variable
        [SerializeField] private int priceAfterAdjustment;      //The price after population price variable adjustment

        [Header("Population")]
        [SerializeField] private IntVariable populationCount;   //The population count
        [SerializeField] private int basePopulation;            //The base population of a charm

        /// <summary>
        /// Adjusting the base population of the charms based on the rarity
        /// </summary>
        void BasePopulationAdjustment()
        {
            switch(itemRarity)
            {
                case ItemRarity.Common:
                    basePopulation = 80;
                    break;

                case ItemRarity.Uncommon:
                    basePopulation = 40;
                    break;

                case ItemRarity.Rare:
                    basePopulation = 20;
                    break;

                case ItemRarity.Epic:
                    basePopulation = 10;
                    break;
            }
        }

        /// <summary>
        /// Calculating price based on population
        /// </summary>
        public void CalculatePrice()
        {

            // I know this looks ugly AF, I'm still figuring out the logic here

            if (populationCount == basePopulation) { populationPriceVariable = 1; };    //Multiplies the price by 1

            if(populationCount <= basePopulation * 0.45)
            {
                if(populationCount <= basePopulation * 0.15)
                {
                    populationPriceVariable = 2f;   //Double the price
                }
                else { populationPriceVariable = 1.5f; }    //Multiplies the price by 1.5
            }

            if(populationCount >= basePopulation * .75)  //This part does not make sense, see notes below
            {
                populationPriceVariable = .5f;  //Reduce the price by a factor of 0.5
            }

            // Notes:
            // Does the base population means the starting population or the reference population?
            // Opt 1. Base population = starting population
            // If it is the base population, then the overpopulation should start above 100%, somewhere around 140?
            // Because, if it starts at 75%, then the charm will default to overpopulation on day 1
            // Opt 2. Base population = reference population
            // If it is just a reference population, then we need to decide another number for the starting population
        }

        public override void OnSell()
        {
            base.OnSell();
        }
    }
}
