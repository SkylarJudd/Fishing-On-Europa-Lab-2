using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOEItem), menuName = "Soap/ScriptableLists/"+ nameof(FOEItem))]
    public class ScriptableListFOEItem : ScriptableList<FOEItem>
    {
        
    }
}
