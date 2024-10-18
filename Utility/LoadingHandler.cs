using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingHandler : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    public void SetSlider(float value)
    {
        slider.value = value;
    }
}
