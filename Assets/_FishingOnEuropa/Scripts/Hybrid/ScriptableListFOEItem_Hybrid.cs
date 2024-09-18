using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOEItem_Hybrid), menuName = "Soap/ScriptableLists/"+ nameof(FOEItem_Hybrid))]
    public class ScriptableListFOEItem_Hybrid : ScriptableList<FOEItem_Hybrid>
    {
        
    }
}
