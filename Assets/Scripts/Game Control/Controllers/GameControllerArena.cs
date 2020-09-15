using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerArena : GameControllerCommon
{

    public float timeLeftSeconds;
    public float minimumDistanceBetweenNewGoals = 40f;
    public Dictionary<GlobalPlayerController, int> scoreboard;

    [SerializeField]
    DestinationPoint[] destinations;
    int currentSelectedPoint;

    Waypoint wp;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        
        gameState = GameState.NOT_STARTED;
        wp = Object.FindObjectOfType<Waypoint>();
        scoreboard = new Dictionary<GlobalPlayerController, int>();
        scoreboard.Clear();

        foreach (GlobalPlayerController player in Object.FindObjectsOfType<GlobalPlayerController>() ) {
            DeactivatePlayer(player);
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
    new void Update()
    {
        base.Update();

        if(gameState == GameState.PLAYING) {
            timeLeftSeconds -= Time.deltaTime;

            if(timeLeftSeconds < 0) {
                gameState = GameState.ENDED;
                DeactivateAllPlayers();
            }
        }
    }

    public void AddPoint(GlobalPlayerController player, int pointCount) {
        if (scoreboard.ContainsKey(player)) {
            scoreboard[player] = scoreboard[player] + pointCount;
        } else {
            scoreboard[player] = pointCount;
        }
    }

    void ActivateAllPlayers() {
        foreach( GlobalPlayerController gpc in scoreboard.Keys) {
            ActivatePlayer(gpc);
        }
    }

       void DeactivateAllPlayers() {
        foreach( GlobalPlayerController gpc in scoreboard.Keys) {
            DeactivatePlayer(gpc);
        }
    }


    void ActivatePlayer(GlobalPlayerController player) {
        player.enabled = true;
        player.EnableDefaultControls();
    }

    void DeactivatePlayer(GlobalPlayerController player) {
        player.DisableAllControls();
        player.enabled = false;
        
    }

    public void SetNewDestination() {

        if (destinations.Length < 2) return; 
        
        int newChoose = currentSelectedPoint;
        float newDist = 0f;
        
        do {
            newChoose = Random.Range(0, destinations.Length);
            newDist = Vector3.Distance(destinations[newChoose].transform.position, destinations[currentSelectedPoint].transform.position);
        } while(newChoose == currentSelectedPoint || newDist < minimumDistanceBetweenNewGoals);

        Debug.Log("New checkpoint distance: " + newDist);
        destinations[currentSelectedPoint].gameObject.SetActive(false); //disable old point
        currentSelectedPoint = newChoose;
        destinations[currentSelectedPoint].gameObject.SetActive(true); //enable new point 
        wp.target = destinations[currentSelectedPoint].transform;
    }

    public override void StartGame() {
        gameState = GameState.PLAYING;
        ActivateAllPlayers();
    }
}
