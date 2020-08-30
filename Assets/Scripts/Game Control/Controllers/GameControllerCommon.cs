using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public virtual void StartGame() {
        gameState = GameState.PLAYING;
    }
}
