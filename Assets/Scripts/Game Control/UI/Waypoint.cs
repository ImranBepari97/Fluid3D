using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    public Image img;
    public Transform target;
    public Vector3 offset;


    public float upSpaceThreshold;
    public float downSpaceThreshold;
    public GameObject upElement;
    public GameObject downElement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 position = Camera.main.WorldToScreenPoint(target.position + offset);

        if (Vector3.Dot(target.position - Camera.main.transform.position, Camera.main.transform.forward) < 0) {
            if (position.x < Screen.width / 2) {
                position.x = maxX;
            } else {
                position.x = minX;
            }
        }

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        float yDist = target.position.y - Camera.main.transform.position.y;
        if (yDist < downSpaceThreshold) {
            downElement.SetActive(true);
            upElement.SetActive(false);
        } else if (yDist > upSpaceThreshold) {
            downElement.SetActive(false);
            upElement.SetActive(true);
        } else {
            downElement.SetActive(false);
            upElement.SetActive(false);
        }

        img.transform.position = position;

    }
}
