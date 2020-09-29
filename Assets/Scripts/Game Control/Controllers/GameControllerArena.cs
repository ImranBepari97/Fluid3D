using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Playables;

public class GameControllerArena : GameControllerCommon
{

    [SyncVar]    
    public float timeLeftSeconds;
    public float minimumDistanceBetweenNewGoals = 40f;

    public int minimumPlayersToStart = 1;

    [System.Serializable]
    public class SyncDictionaryScoreboard : SyncDictionary<NetworkIdentity, int> {}
    public SyncDictionaryScoreboard scoreboard;

    [SerializeField]
    DestinationPoint[] destinations;
    int currentSelectedPoint;

    public PlayableDirector localAnimatorCutscene; 

    NetworkManager nm; 

    Waypoint wp;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        

        gameState = GameState.WAITING_FOR_PLAYERS;
        
        scoreboard = new SyncDictionaryScoreboard();
        scoreboard.Clear();
    }

    void Start() {
        nm = NetworkManager.singleton;
        wp = Object.FindObjectOfType<Waypoint>();
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
        if(!NetworkServer.dontListen) {
            if(nm.numPlayers > minimumPlayersToStart && gameState == GameState.WAITING_FOR_PLAYERS) {
                gameState = GameState.NOT_STARTED; //begin countdown since we have enough players
            }

            if(!(nm.numPlayers > minimumPlayersToStart) && gameState == GameState.WAITING_FOR_PLAYERS ) { //&& condition to enforce that the game hasnt started yet
                return;
            }
        } else {
            if(GlobalPlayerController.localInstance != null && gameState == GameState.WAITING_FOR_PLAYERS) {
                gameState = GameState.NOT_STARTED;
                scoreboard.Add(GlobalPlayerController.localInstance.gameObject.GetComponent<NetworkIdentity>(), 0);
                DeactivateAllPlayers();
                localAnimatorCutscene.Play();
                Debug.Log("Only player has joined game offline, starting");
            }
        }

        if(gameState == GameState.WAITING_FOR_PLAYERS) {
            return;
        }

        base.Update();

        if(gameState == GameState.PLAYING) {
            timeLeftSeconds -= Time.deltaTime;

            if(timeLeftSeconds < 0) {
                gameState = GameState.ENDED;
                DeactivateAllPlayers();
            }
        }
    }

    public void AddPoint(NetworkIdentity player, int pointCount) {
        if (scoreboard.ContainsKey(player)) {
            scoreboard[player] = scoreboard[player] + pointCount;
        } else {
            scoreboard[player] = pointCount;
        }
    }

    public void ActivateAllPlayers() {
        foreach( NetworkIdentity player in scoreboard.Keys) {
            GlobalPlayerController gpc = player.gameObject.GetComponent<GlobalPlayerController>();
            ActivatePlayer(gpc);
        }
    }

    public void DeactivateAllPlayers() {
        foreach( NetworkIdentity player in scoreboard.Keys) {
            GlobalPlayerController gpc = player.gameObject.GetComponent<GlobalPlayerController>();
            DeactivatePlayer(gpc);
        }
    }


    public void ActivatePlayer(GlobalPlayerController player) {
        player.enabled = true;
        player.EnableDefaultControls();
    }

    public void DeactivatePlayer(GlobalPlayerController player) {
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
