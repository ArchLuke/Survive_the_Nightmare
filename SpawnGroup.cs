using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SpawnGroup : MonoBehaviour
{
    [SerializeField] private GameObject _group;
    private void OnTriggerEnter(Collider other)
    {
        _group.SetActive(true);
    }
}
