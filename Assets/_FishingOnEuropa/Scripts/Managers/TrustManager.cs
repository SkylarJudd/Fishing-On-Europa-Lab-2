using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class TrustManager : Singleton<TrustManager>
    {
        [SerializeField] int pat_IncreaseTrust, feed_IncreaseTrust, favFeed_IncreaseTrust, plushie_IncreaseTrust;
        [SerializeField] int feed_DecreaseTrust, charmNotCollected_DecreaseTrust;


        //subcribe to end of day
        public void EndOfDayCheck()
        {
            foreach (var item in _FNAVM._hybridsTamedList)
            {
                UpdateHybridTrustEndOfDay(item.FOEHybridSaveData);
            }
        }

        void UpdateHybridTrustEndOfDay(FOESaveItem_Hybrid _FOESaveItem_Hybrid)
        {
            //check if trust increasing actions have been done

            //if they haven't been fed or charge hasn't been collected, decrease trust
            if (!_FOESaveItem_Hybrid.charmCollectedCurrentDay)
                _FOESaveItem_Hybrid.hybridTrust -= charmNotCollected_DecreaseTrust;
            if (!_FOESaveItem_Hybrid.hasBeenFedCurrentDay)
                _FOESaveItem_Hybrid.hybridTrust -= feed_DecreaseTrust;

            //reset trust increasing value action trackers and save them
            _FOESaveItem_Hybrid.hasBeenPatCurrentDay = false;
            _FOESaveItem_Hybrid.hasBeenFedCurrentDay = false;
            _FOESaveItem_Hybrid.hasBeenFedCurrentDay = false;
            _FOESaveItem_Hybrid.charmCollectedCurrentDay = false;

        }

        public void InitaliseHybridCharm()
        {

        }

        /// <summary>
        /// Increase trust when Hybrid is pat
        /// </summary>
        /// <param name="_itemHybrid"></param>
        public void UpdateHybridPatTrust(FOEItem_Hybrid _itemHybrid)
        {
            if(!_itemHybrid.FOEHybridSaveData.hasBeenPatCurrentDay)
            {
                _itemHybrid.FOEHybridSaveData.hasBeenPatCurrentDay = true;

                //increase trust
                _itemHybrid.FOEHybridSaveData.hybridTrust += pat_IncreaseTrust;
            }
           
        }

        /// <summary>
        /// Increase trust when Hybrid is near plushie
        /// </summary>
        /// <param name="_itemHybrid"></param>
        public void UpdateHybridPlushieTrust(FOEItem_Hybrid _itemHybrid)
        {
            if (!_itemHybrid.FOEHybridSaveData.hasHadPlushieCurrentDay)
            {

                _itemHybrid.FOEHybridSaveData.hasHadPlushieCurrentDay = true;

                //increase trust
                _itemHybrid.FOEHybridSaveData.hybridTrust += plushie_IncreaseTrust;

            }

        }
        
        /// <summary>
        /// Increase trust when Hybrid is fed
        /// </summary>
        /// <param name="_itemHybrid"></param>
        public void UpdateHybridFeedTrust(FOEItem_Hybrid _itemHybrid, bool isFavFood)
        {
            if (!_itemHybrid.FOEHybridSaveData.hasBeenFedCurrentDay)
            {
                _itemHybrid.FOEHybridSaveData.hasBeenFedCurrentDay = true;

                //increase trust
                if(isFavFood)
                    _itemHybrid.FOEHybridSaveData.hybridTrust += favFeed_IncreaseTrust;
                else
                    _itemHybrid.FOEHybridSaveData.hybridTrust += feed_IncreaseTrust;

            }

        }
    }
}
