using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BasicUI : MonoBehaviour
{
    GameController gc;

    public GlobalPlayerController currentPlayer;
    public TMP_Text timer;
    public TMP_Text score;
    


    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score: " + gc.scoreboard[currentPlayer];
        timer.text = returnTimerAsText(gc.timeLeftSeconds);
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
