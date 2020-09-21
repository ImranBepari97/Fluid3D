using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{

    public static bool isPaused = false; 

    public bool isPauseView;
    public GameObject pauseMenuUIParent;

    public GameObject defaultPauseMenu;
    
    public GameObject quitConfirmation;

    // Start is called before the first frame update
    void Awake() {
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        isPauseView = PauseMenu.isPaused;
        if(Input.GetButtonDown("Pause") && GameControllerCommon.instance != null && GameControllerCommon.instance.gameState != GameState.ENDED) {
            if(isPaused) {
                Unpause();
            } else {
                Pause();
            }
        }
    }


    public void ToggleConfirmation(bool show) {
        quitConfirmation.SetActive(show);
        defaultPauseMenu.SetActive(!show);

        Selectable first;

        if(show) {
            first = quitConfirmation.GetComponentInChildren<Selectable>();
        } else {
            first = defaultPauseMenu.GetComponentInChildren<Selectable>();
        }

        first.Select();
        first.OnSelect(null);
    }

    public void Unpause() {
        pauseMenuUIParent.SetActive(false);
        quitConfirmation.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause() {
        Debug.Log("Pause called");
        pauseMenuUIParent.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Selectable first = pauseMenuUIParent.GetComponentInChildren<Selectable>();
        first.Select();
        first.OnSelect(null);
    }
}
