using UnityEngine;
using Obvious.Soap;
using System;

namespace Europa
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOEItem_Crop), menuName = "Soap/ScriptableLists/" + nameof(FOEItem_Crop))]
    public class ScriptableListCropData : ScriptableList<FOEItem_Crop>
    {
        public event Action<FOEItem_Crop> OnGrothStageUpdate;

        public void UpdateGrothStage(FOEItem_Crop _Crop)
        {
            OnGrothStageUpdate.Invoke(_Crop);

        }

    }
}

