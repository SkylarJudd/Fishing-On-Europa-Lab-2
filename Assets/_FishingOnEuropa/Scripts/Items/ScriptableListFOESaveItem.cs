using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOESaveItem), menuName = "Soap/ScriptableLists/"+ nameof(FOESaveItem))]
    public class ScriptableListFOESaveItem : ScriptableList<FOESaveItem>
    {
        
    }
}
