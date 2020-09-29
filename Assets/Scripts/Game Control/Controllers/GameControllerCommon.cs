using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameControllerCommon : NetworkBehaviour
{

    public float countdownLeft = 3f;
    public GameState gameState;

    // Start is called before the first frame update

    public static GameControllerCommon instance;
    public void Awake()
    {
        if(instance != null ) {
            Debug.Log("Another GameController already exists, deleting this one.");
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        PauseMenu.isPaused = false;
    }

    // Update is called once per frame
    public void Update()
    {
        StartCountdown();


        if(PauseMenu.isPaused) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(gameState == GameState.ENDED) {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void StartCountdown() {
        if(gameState == GameState.NOT_STARTED) {
            countdownLeft -= Time.deltaTime;

            if(countdownLeft < 0) {
                StartGame();  
            }
        }

        if(Input.GetButtonDown("Restart")) {
            RestartLevel();
        }
    }

    public void ToggleCameraControls(bool active) {
        CameraRig.instance.enabled = active;
    }

    public virtual void StartGame() {
        gameState = GameState.PLAYING;
    }

    public void RestartLevel() {
        LevelTransitionLoader.instance.LoadSceneWithTransition(SceneManager.GetActiveScene().name);

        if(OfflineNetworkManager.instance != null) {
            OfflineNetworkManager.instance.StopHost();
        }
    }

    public void QuitToMenu() {
        LevelTransitionLoader.instance.LoadSceneWithTransition("MainMenu");

        if(OfflineNetworkManager.instance != null) {
            OfflineNetworkManager.instance.StopHost();
        }
    }
}
