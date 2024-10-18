using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Destination : MonoBehaviour
{
    public List<Transform> points = null;
    public SharedInt destinationPoint = null;
    public SharedVector3 playerPosition = null;
    public SharedVector3 playerDirection = null;
    public GameObject destination;
    public GameObject text;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (points[destinationPoint.value] == null)
        {
            destination.SetActive(false);
            text.SetActive(false);
            return;
        }
        Camera cam = Camera.main;
        Vector3 screenPos = cam.WorldToScreenPoint(points[destinationPoint.value].position);
        Vector3 direction = -(playerPosition.value - points[destinationPoint.value].position);
        float angle = Mathf.Abs(Vector3.Angle(direction, playerDirection.value));
        
        
        if (angle > 90)
        {
            destination.SetActive(false);
            text.SetActive(false);
        }
        else
        {
            destination.SetActive(true);
            text.SetActive(true);
        }
        
        destination.GetComponent<RectTransform>().position = screenPos;
        float distance = Vector3.Distance(playerPosition.value, points[destinationPoint.value].position);
        
        if (distance < 1f)
        {
            destination.SetActive(false);
            text.SetActive(false);
        }

        text.GetComponent<TextMeshProUGUI>().text = (distance).ToString("#.0") + 'm';
    }
}
