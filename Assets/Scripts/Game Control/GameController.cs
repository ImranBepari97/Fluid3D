using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public float timeLeftSeconds;
    public Dictionary<GlobalPlayerController, int> scoreboard;

    public bool isGameOver;
    [SerializeField]
    DestinationPoint[] destinations;
    int currentSelectedPoint;

    Waypoint wp;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        wp = Object.FindObjectOfType<Waypoint>();
        scoreboard = new Dictionary<GlobalPlayerController, int>();
        scoreboard.Clear();

        foreach (GlobalPlayerController player in Object.FindObjectsOfType<GlobalPlayerController>() ) {
            scoreboard.Add(player, 0);
        }

        
        destinations = Object.FindObjectsOfType<DestinationPoint>();
        currentSelectedPoint = Random.Range(0, destinations.Length);

        foreach (DestinationPoint d in destinations) {
            d.gameObject.SetActive(false);
        }

        destinations[currentSelectedPoint].gameObject.SetActive(true);
        wp.target = destinations[currentSelectedPoint].transform;

    }

    // Update is called once per frame
    void Update()
    {

        if(!isGameOver) {
            timeLeftSeconds -= Time.deltaTime;
        }

        if(timeLeftSeconds < 0) {
            isGameOver = true;
        }
    }

    public void AddPoint(GlobalPlayerController player, int pointCount) {
        if (scoreboard.ContainsKey(player)) {
            scoreboard[player] = scoreboard[player] + pointCount;
        } else {
            scoreboard[player] = pointCount;
        }
    }

    public void SetNewDestination() {

        if (destinations.Length < 2) return; 


        int newChoose = currentSelectedPoint;
        
        while(newChoose == currentSelectedPoint) {
            newChoose = Random.Range(0, destinations.Length);
        }

        destinations[currentSelectedPoint].gameObject.SetActive(false); //disable old point
        currentSelectedPoint = newChoose;
        destinations[currentSelectedPoint].gameObject.SetActive(true); //enable new point 
        wp.target = destinations[currentSelectedPoint].transform;
    }
}
