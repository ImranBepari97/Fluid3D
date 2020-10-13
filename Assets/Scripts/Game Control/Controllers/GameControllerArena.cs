using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Playables;
using System.Linq;

public class GameControllerArena : GameControllerCommon {

    [SyncVar]
    public float timeLeftSeconds;
    public float minimumDistanceBetweenNewGoals = 40f;

    [System.Serializable]
    public class SyncDictionaryScoreboard : SyncDictionary<NetworkIdentity, int> { }
    public SyncDictionaryScoreboard scoreboard = new SyncDictionaryScoreboard();

    [SerializeField]
    DestinationPoint destination;

    [SerializeField]
    Vector3[] destinationSpawns;

    [SyncVar]
    int currentSelectedPoint;

    public PlayableDirector localAnimatorCutscene;

    Waypoint wp;

    // Start is called before the first frame update

    public override void OnStartServer() {
        gameState = GameState.WAITING_FOR_PLAYERS;
        scoreboard.Clear();

        //Get all destinations
        GameObject[] destSpawn = GameObject.FindGameObjectsWithTag("DestinationPoint");
        destinationSpawns = new Vector3[destSpawn.Length];
        for (int i = 0; i < destSpawn.Length; i++) {
            destinationSpawns[i] = destSpawn[i].transform.position;
        }

        //set first destination point
        currentSelectedPoint = Random.Range(0, destinationSpawns.Length);
        destination.transform.position = destinationSpawns[currentSelectedPoint];
    }

    new void Awake() {
        base.Awake();
    }

    // Update is called once per frame
    new void Update() {

        if (!NetworkServer.dontListen) {
            if (isServer) {
                //IF ONLINE
                if (gameState == GameState.WAITING_FOR_PLAYERS) {
                    if ((NetworkManager.singleton as MainRoomManager).playersToGoIngame.Count == 0) {
                        gameState = GameState.NOT_STARTED; //begin countdown since we have enough players
                        RpcAllPlayCutscene();
                        Debug.Log("Starting");
                    } else {
                        return;
                    }
                }
            }
        } else {
            //IF OFFLINE
            if (GlobalPlayerController.localInstance != null && gameState == GameState.WAITING_FOR_PLAYERS) {

                Debug.Log("chjecking if player is on leaderboard");
                if (!scoreboard.ContainsKey(GlobalPlayerController.localInstance.gameObject.GetComponent<NetworkIdentity>())) {
                    Debug.Log("Not on leaderboard");
                    return; //wait until the player has spawned
                }
                
                gameState = GameState.NOT_STARTED;
                DeactivateAllPlayers();
                localAnimatorCutscene.Play();
                Debug.Log("Only player has joined game offline, starting");
            }
        }

        if (gameState == GameState.WAITING_FOR_PLAYERS) {
            return;
        }

        base.Update();

        if (gameState == GameState.PLAYING) {
            timeLeftSeconds -= Time.deltaTime;

            if (timeLeftSeconds < 0) {
                gameState = GameState.ENDED;

                if (isServer) {
                    DeactivateAllPlayers();
                    StartCoroutine(GoBackToLobbyRoutine());
                }
            }
        }
    }

    [Server]
    public IEnumerator GoBackToLobbyRoutine() {
        yield return new WaitForSeconds(5f);
        GoBackToLobby();
    }

    [Server]
    public void GoBackToLobby() {
        MainRoomManager mrm = (NetworkManager.singleton as MainRoomManager);
        mrm.ServerChangeScene(mrm.GetRoomScene());
    }

    [ClientRpc]
    public void RpcAllPlayCutscene() {
        Debug.Log("Play cutscene");
        localAnimatorCutscene.Play();
    }

    [Server]
    public void AddPoint(NetworkIdentity player, int pointCount) {
        if (scoreboard.ContainsKey(player)) {
            scoreboard[player] = scoreboard[player] + pointCount;
        } else {
            scoreboard[player] = pointCount;
        }
    }

    [Server]
    public void ActivateAllPlayers() {
        foreach (NetworkIdentity player in scoreboard.Keys) {
            GlobalPlayerController gpc = player.gameObject.GetComponent<GlobalPlayerController>();
            ActivatePlayer(gpc);
        }
    }

    [Server]
    public void DeactivateAllPlayers() {
        foreach (NetworkIdentity player in scoreboard.Keys) {
            GlobalPlayerController gpc = player.gameObject.GetComponent<GlobalPlayerController>();
            DeactivatePlayer(gpc);
        }
    }

    [Server]
    public void AddPlayerToScoreboard(NetworkIdentity playerToAdd) {
        scoreboard.Add(playerToAdd, 0);
    }

    [Server]
    public void ActivatePlayer(GlobalPlayerController player) {
        player.enabled = true;
        player.EnableDefaultControls();
        RpcClientActivatePlayer(player.GetComponent<NetworkIdentity>());

    }

    [Server]
    public void DeactivatePlayer(GlobalPlayerController player) {
        player.DisableAllControls();
        player.enabled = false;
        RpcClientDeactivatePlayer(player.GetComponent<NetworkIdentity>());

    }

    [ClientRpc]
    public void RpcClientDeactivatePlayer(NetworkIdentity player) {
        GlobalPlayerController gpc = player.GetComponent<GlobalPlayerController>();
        gpc.DisableAllControls();
        gpc.enabled = false;
    }

    [ClientRpc]
    public void RpcClientActivatePlayer(NetworkIdentity player) {
        Debug.Log("Activating " + player);
        GlobalPlayerController gpc = player.GetComponent<GlobalPlayerController>();
        gpc.enabled = true;
        gpc.EnableDefaultControls();
    }

    [Server]
    public void SetNewDestination() {

        if (destinationSpawns.Length < 2) return;

        int newChoose = currentSelectedPoint;
        float newDist = 0f;

        do {
            newChoose = Random.Range(0, destinationSpawns.Length);
            newDist = Vector3.Distance(destinationSpawns[newChoose], destinationSpawns[currentSelectedPoint]);
        } while (newChoose == currentSelectedPoint || newDist < minimumDistanceBetweenNewGoals);

        Debug.Log("New checkpoint distance: " + newDist);
        currentSelectedPoint = newChoose;
        destination.transform.position = destinationSpawns[currentSelectedPoint];
    }

    [Server]
    public override void StartGame() {
        gameState = GameState.PLAYING;
        Debug.Log("Starting Game and activating players");
        ActivateAllPlayers();
    }
}
