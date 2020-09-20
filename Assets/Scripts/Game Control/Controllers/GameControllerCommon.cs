using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerCommon : MonoBehaviour
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
    }

    // Update is called once per frame
    public void Update()
    {
        StartCountdown();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
