using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
        isPauseView = isPaused;
        if(Input.GetButtonDown("Pause")) {
            if(GameControllerCommon.instance == null) {
                CheckPause();
            } else if(GameControllerCommon.instance.gameState != GameState.ENDED) {
                CheckPause();
            }
        }

        if (isPaused) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void CheckPause() {
        if(isPaused) {
            Unpause();
        } else {
            Pause();
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

    public void QuitToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
