using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{

     GameControllerTimeTrial gc;
    // Start is called before the first frame update
    void Start()
    {
        gc = (GameControllerTimeTrial) GameControllerCommon.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.GetComponent<GlobalPlayerController>()) {
            gc.gameState = GameState.ENDED;
        }
    }
}
