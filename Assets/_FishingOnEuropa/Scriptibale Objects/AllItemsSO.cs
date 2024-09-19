using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [CreateAssetMenu(fileName = "ItemList", menuName = "Europa/Item/AllItems", order = 1)]
    public class AllItemsSO : ScriptableObject
    {
        public List<FOEItem> items;

    }
}
