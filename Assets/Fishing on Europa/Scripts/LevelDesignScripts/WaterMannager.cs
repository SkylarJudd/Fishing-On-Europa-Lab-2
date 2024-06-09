using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMannager : MonoBehaviour
{
    public float wavesHight = 7f;
    public float wavesFrequency = 1f;
    public float waveSpeed = 4f;
    public Transform water;

    Material waterMat;
    Texture2D waveDisplacement;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetVaribles()
    {
        waterMat = water.GetComponent<Renderer>().sharedMaterial;
        waveDisplacement = (Texture2D)waterMat.GetTexture("_WavesDisplacment");
    }

    public float WaterHightAtPosition(Vector3 position)
    {
        return water.position.y + waveDisplacement.GetPixelBilinear(position.x * wavesFrequency/100, position.z * wavesFrequency/100 + Time.time * waveSpeed/100).g * wavesHight/100 * water.localScale.x;
    }

    private void OnValidate()
    {
        if (!waterMat)
            SetVaribles();

        updateMaterial();
            
    }

    void updateMaterial()
    {
        waterMat.SetFloat("_WaveFrequency", wavesFrequency/100);
        waterMat.SetFloat("_WaveSpeed", waveSpeed/100);
        waterMat.SetFloat("_WaveHight", wavesHight/100);
    }
}
