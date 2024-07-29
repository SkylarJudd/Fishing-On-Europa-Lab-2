using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToyList { MussleHands, HypnoGlasses, CarrotPlushie, BallOfYarn, BonePlushie, HoneyJarPlushie, Crown , LeafPlushie , BambooPlushie , NotDecided};
[CreateAssetMenu(fileName = "Toys", menuName = "Europa/Toys", order = 1)]

public class Toys : ScriptableObject
{
    public ToyList toyName;
    public int happynessAdd;
}
