using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;


public class GameControllerTimeTrial : GameControllerCommon
{
     public float timerSeconds;

     GlobalPlayerController gpc;

     public string nameOfNextLevel;

     public PlayableDirector localAnimatorCutscene; 

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();

        timerSeconds = 0f;
        gameState = GameState.NOT_STARTED;
    }

    // Update is called once per frame
    new void Update()
    {
        if(gpc == null) {
            if(GlobalPlayerController.localInstance != null) {
                gpc = GlobalPlayerController.localInstance;
                gpc.DisableAllControls();
                gpc.enabled = false;
                localAnimatorCutscene.Play();
            } else {
                //This should not move until the player exists
                return;
            }
        }

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
        if(nameOfNextLevel != null) {
            LevelTransitionLoader.instance.LoadSceneWithTransition(nameOfNextLevel);

            if(OfflineNetworkManager.instance != null) {
                OfflineNetworkManager.instance.StopHost();
            }
        }
    }
}
