using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Mirror;

public class ArenaUI : MonoBehaviour
{
    GameControllerArena gc;

    NetworkIdentity localPlayer;
    public TMP_Text timer;
    public TMP_Text score;

    public GameObject resultScreen;
    public TMP_Text resultScore;

    
    // Start is called before the first frame update
    void Start()
    {
        gc = (GameControllerArena) GameControllerCommon.instance;
        timer.text = returnTimerAsText(gc.timeLeftSeconds);
    }

    // Update is called once per frame
    void Update()
    {

        if(localPlayer == null) {
            if(NetworkClient.isConnected && GlobalPlayerController.localInstance != null) {
                localPlayer = GlobalPlayerController.localInstance.gameObject.GetComponent<NetworkIdentity>();
            }
        } else if(gc.scoreboard.ContainsKey(localPlayer)) {
            score.text = "Score: " + gc.scoreboard[localPlayer];
            timer.text = returnTimerAsText(gc.timeLeftSeconds);

            if(gc.gameState == GameState.ENDED && !resultScreen.activeInHierarchy) {
                resultScreen.SetActive(true);
                resultScore.text = "" + gc.scoreboard[localPlayer];
                gc.ToggleCameraControls(false);
                Cursor.lockState = CursorLockMode.None;

                Button firstButton = resultScreen.GetComponentInChildren<Button>();
                firstButton.Select();
                firstButton.OnSelect(null);
            }   
        }
    }

    string returnTimerAsText(float timeInSeconds) {

        if(timeInSeconds < 0) {
            return "00:00";
        }

        int timeInSecondsAsInt = (int)timeInSeconds;
        int seconds = timeInSecondsAsInt % 60;
        string secForm = seconds > 9 ? seconds + "" : "0" + seconds;

        int minutes = timeInSecondsAsInt / 60;
        string minForm = minutes > 9 ? minutes + "" : "0" + minutes;

        return minForm + ":" + secForm;

    }
}
