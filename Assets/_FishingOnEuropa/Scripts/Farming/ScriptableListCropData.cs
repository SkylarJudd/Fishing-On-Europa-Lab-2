using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOEItem_Crop), menuName = "Soap/ScriptableLists/" + nameof(FOEItem_Crop))]
    public class ScriptableListCropData : ScriptableList<FOEItem_Crop>
    {

    }
}

