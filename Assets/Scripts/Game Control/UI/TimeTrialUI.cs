using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeTrialUI : MonoBehaviour
{
    GameControllerTimeTrial gc;
    public TMP_Text timer;

    // Start is called before the first frame update
    void Start()
    {
        gc = (GameControllerTimeTrial) GameControllerCommon.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timer.text = returnTimerAsText(gc.timerSeconds);
    }

    string returnTimerAsText(float timeInSeconds) {
        int intTime = (int) timeInSeconds;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = timeInSeconds * 1000;
        fraction = (fraction % 1000);
        
        string timeText = string.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    
    }
}
