using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerTimeTrial : GameControllerCommon
{
     public float timerSeconds;

     GlobalPlayerController gpc;

     public string nameOfNextLevel;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();

        timerSeconds = 0f;
        gameState = GameState.NOT_STARTED;
        gpc = GameObject.FindObjectOfType<GlobalPlayerController>();

        gpc.DisableAllControls();
        gpc.enabled = false;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if(gameState == GameState.PLAYING) {
            timerSeconds += Time.deltaTime;
        }

        if(gameState == GameState.ENDED) {
            gpc.DisableAllControls();
            gpc.enabled = false;
        }
    }

    public override void StartGame() {
        gameState = GameState.PLAYING;
        gpc.enabled = true;
        gpc.EnableDefaultControls();
    }

    public void LoadNextLevel() {
        if(nameOfNextLevel != null && Application.CanStreamedLevelBeLoaded(nameOfNextLevel)) {
            LevelTransitionLoader.instance.LoadSceneWithTransition(nameOfNextLevel);
        }
    }
}
