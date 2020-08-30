using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTimeTrial : GameControllerCommon
{
     public float timerSeconds;

     GlobalPlayerController gpc;

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
    }

    public override void StartGame() {
        gameState = GameState.PLAYING;
        gpc.enabled = true;
        gpc.EnableDefaultControls();
    }
}
