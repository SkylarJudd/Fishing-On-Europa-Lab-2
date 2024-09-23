using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class TrustManager : Singleton<TrustManager>
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }


        //subcribe to end of day
        void EndOfDayCheck(FOESaveItem_Hybrid _FOESaveItem_Hybrid)
        {
            //check if trust increasing actions have been done
            //if they haven't decrease trust

            //reset trust increasing value action trackers

            //save
            _FOESaveItem_Hybrid.hasBeenPatCurrentDay = false;
            _FOESaveItem_Hybrid.hasBeenFedCurrentDay = false;
            _FOESaveItem_Hybrid.hasBeenFedCurrentDay = false;

        }

        public void UpdateHybridPat()
        {

        }
    }
}
