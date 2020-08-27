using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{


    public float countdownLeft = 3f;
    public float timeLeftSeconds;
    public Dictionary<GlobalPlayerController, int> scoreboard;

    public GameState gameState;
    [SerializeField]
    DestinationPoint[] destinations;
    int currentSelectedPoint;

    public static GameController instance;

    Waypoint wp;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null ) {
            Debug.Log("Another GameController already exists, deleting this one.");
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        
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
    void Update()
    {
        if(gameState == GameState.PLAYING) {
            timeLeftSeconds -= Time.deltaTime;

            if(timeLeftSeconds < 0) {
                gameState = GameState.ENDED;
                DeactivateAllPlayers();
            }
        }

        if(gameState == GameState.NOT_STARTED) {
            countdownLeft -= Time.deltaTime;

            if(countdownLeft < 0) {
                gameState = GameState.PLAYING;
                ActivateAllPlayers();
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
        
        while(newChoose == currentSelectedPoint) {
            newChoose = Random.Range(0, destinations.Length);
        }

        destinations[currentSelectedPoint].gameObject.SetActive(false); //disable old point
        currentSelectedPoint = newChoose;
        destinations[currentSelectedPoint].gameObject.SetActive(true); //enable new point 
        wp.target = destinations[currentSelectedPoint].transform;
    }
}
