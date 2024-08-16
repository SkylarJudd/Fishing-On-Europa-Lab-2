using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOEItem_Seed : FOEItem
{
    public SeedsSO seedsSO;
    [Header("Visuals")]
    [SerializeField]
    private GameObject inWorldVisuals;
    [SerializeField]
    private GameObject inHandVisuals;
    [SerializeField]
    private MeshRenderer bubbleVisuals;

    public override void OnDrop(Hand _Hand, Grabbable _Grabbable)
    {
        base.OnDrop(_Hand, _Grabbable);

        ToggleHandVisuals(false);
    }
    public override void OnPickUp(Hand _Hand, Grabbable _Grabbable)
    {
        ToggleHandVisuals(true);
    }
    private void ToggleHandVisuals(bool _Toggle)
    {
        inHandVisuals.SetActive(!_Toggle);
        inWorldVisuals.SetActive(_Toggle);
        bubbleVisuals.enabled = _Toggle;
    }
}
