using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Dictionary<GameObject, int> scoreboard;
    DestinationPoint[] destinations;
    int currentSelectedPoint;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard.Clear();
        destinations = Object.FindObjectsOfType<DestinationPoint>();
        currentSelectedPoint = Random.Range(0, destinations.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoint(GameObject player, int pointCount) {
        if (scoreboard.ContainsKey(player)) {
            scoreboard[player] = scoreboard[player] + pointCount;
        } else {
            scoreboard[player] = pointCount;
        }
    }

    public void SetNewDestination() {
        int newChoose = currentSelectedPoint;
        
        while(newChoose == currentSelectedPoint) {
            newChoose = Random.Range(0, destinations.Length);
        }

        destinations[currentSelectedPoint].enabled = false; //disable old point
        currentSelectedPoint = newChoose;
        destinations[currentSelectedPoint].enabled = true; //enable new point 
    }
}
