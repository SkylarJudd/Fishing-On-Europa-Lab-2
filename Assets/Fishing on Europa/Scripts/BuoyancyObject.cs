
using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Rigidbody))]

public class BuoyancyObject : MonoBehaviour
{
    [SerializeField] Transform[] floaters;
    [SerializeField] float underWaterDrag = 3f;
    [SerializeField] float underWaterAngularDrag = 1f;
    [SerializeField] float airDrag = 0f;
    [SerializeField] float airAngularDrag = 0.05f;
    [SerializeField] float floatingPower = 15f;
    [SerializeField] TextMeshProUGUI Text;

    [SerializeField] WaterMannager waterManager;

    Rigidbody m_Rigidbody;

    bool underwater;

    int floatersUnderWater;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_Rigidbody == null)
        {
            Text.text = "NOT WORKING HERE";
        }


        floatersUnderWater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float difference = floaters[i].position.y - waterManager.WaterHightAtPosition(floaters[i].position);

            if (difference < 0)
            {
                m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);
                floatersUnderWater += 1;
                if (!underwater)
                {
                    underwater = true;
                    SwitchState(true);
                }
            }
        }


        if (underwater && floatersUnderWater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterDrag;
        }
        else
        {
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airDrag;
        }
    }
}
