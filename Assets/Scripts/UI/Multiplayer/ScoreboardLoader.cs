using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Mirror;

public class ScoreboardLoader : MonoBehaviour {
    [SerializeField] List<PlayerScoreEntry> playerScores;
    [SerializeField] GameObject scoreboardObj;

    GameControllerArena gameController;

    void Start() {
        gameController = GameControllerCommon.instance as GameControllerArena;
        gameController.scoreboard.Callback += OnScoreChange;
    }

    private void Update() {
        if (Input.GetButtonDown("ShowLeaderboard")) {
            scoreboardObj.SetActive(true);
        }

        if (Input.GetButtonUp("ShowLeaderboard")) {
            scoreboardObj.SetActive(false);
        }
    }

    void OnScoreChange(SyncDictionary<NetworkIdentity, int>.Operation op, NetworkIdentity key, int item) {
        RefreshScoreboard();
    }

    void RefreshScoreboard() {
        List<KeyValuePair<NetworkIdentity, int>> sortedList = gameController.scoreboard.ToList();
        sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

        //Clear the board
        foreach(PlayerScoreEntry pse in playerScores) {
            pse.SetDisplayName("");
            pse.SetScore("");
            pse.SetRank("");
        }

        for(int i = 0; i < sortedList.Count; i++) {
            playerScores[i].SetDisplayName(sortedList[i].Key.GetComponent<GamePlayerEntity>().displayName);
            playerScores[i].SetRank("" + (i + 1));
            playerScores[i].SetScore(sortedList[i].Value + "");
        }
    }

}
