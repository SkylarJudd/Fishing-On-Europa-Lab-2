using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(CropData), menuName = "Soap/ScriptableLists/" + nameof(CropData))]
    public class ScriptableListCropData : ScriptableList<CropData>
    {

    }
}

