using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int collectibleID; // Unique ID for this collectible

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
