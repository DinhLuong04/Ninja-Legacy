using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCleaner : MonoBehaviour
{
    private void Start()
    {
        GameObject[] ddolObjects = GameObject.FindGameObjectsWithTag("DDOL");

        foreach (GameObject obj in ddolObjects)
        {
            Destroy(obj);
        }
    }
}
