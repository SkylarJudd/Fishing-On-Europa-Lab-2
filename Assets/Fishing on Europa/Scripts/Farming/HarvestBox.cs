using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestBox : MonoBehaviour
{
    public GameObject apple;
    public GameObject banana;
    public GameObject carrot;
    public GameObject orange;
    public GameObject potato;
    public GameObject watermelon;

    public int appleCount;
    public int bananaCount;
    public int carrotCount;
    public int orangeCount;
    public int potatoCount;
    public int watermelonCount;

    public void Start()
    {
        apple.SetActive(false);
        banana.SetActive(false);
        carrot.SetActive(false);
        orange.SetActive(false);
        potato.SetActive(false);
        watermelon.SetActive(false);
    }

    public void Update()
    {
        if (appleCount > 0)
            apple.SetActive(true);
        else
            apple.SetActive(false);

        if (bananaCount > 0)
            banana.SetActive(true);
        else
            banana.SetActive(false);

        if (carrotCount > 0)
            carrot.SetActive(true);
        else
            carrot.SetActive(false);

        if (orangeCount > 0)
            orange.SetActive(true);
        else
            orange.SetActive(false);

        if (potatoCount > 0)
            potato.SetActive(true);
        else
            potato.SetActive(false);

        if (watermelonCount > 0)
            watermelon.SetActive(true);
        else
            watermelon.SetActive(false);

    }

    /// <summary>
    /// Increases the count of individual fruit when called.
    /// </summary>
    /// <param name="_name">seed scriptable object name of frut to add</param>
    public void AddHarvestCount(string _name)
    {
        if (_name == "Apple")
        {
            appleCount++;
        }

        if (_name == "Banana")
        {
            bananaCount++;
        }

        if (_name == "Carrot")
        {
            carrotCount++;
        }

        if (_name == "Orange")
        {
            orangeCount++;
        }

        if (_name == "Potato")
        {
            potatoCount++;
        }

        if (_name == "Watermelon")
        {
            watermelonCount++;
        }

        else
            return;

    }
}
