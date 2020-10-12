using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TimeTrialUI : MonoBehaviour
{
    GameControllerTimeTrial gc;
    public TMP_Text timer;

    public GameObject resultScreen;
    public TMP_Text resultScore;

    public Button nextLevelButton;


    // Start is called before the first frame update
    void Start()
    {
        gc = (GameControllerTimeTrial) GameControllerCommon.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timer.text = returnTimerAsText(gc.timerSeconds);

        if(gc.gameState == GameState.ENDED && !resultScreen.activeInHierarchy) {
            resultScreen.SetActive(true);
            resultScore.text = returnTimerAsText(gc.timerSeconds);
            gc.ToggleCameraControls(false);
            Cursor.lockState = CursorLockMode.None;

            //disable the next level button if there's no next level set
            if( !(gc.nameOfNextLevel != null && Application.CanStreamedLevelBeLoaded(gc.nameOfNextLevel))) {
                nextLevelButton.gameObject.SetActive(false);
            }

            Button firstButton = resultScreen.GetComponentInChildren<Button>();
            firstButton.Select();
            firstButton.OnSelect(null);
        }
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
