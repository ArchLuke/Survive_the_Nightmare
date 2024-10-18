using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationMarker : MonoBehaviour
{
    public SharedInt destination = null;

    private bool set = false;
    void OnTriggerEnter()
    {
        if (!set)
            destination.value++;
        set = true;
    }
}
