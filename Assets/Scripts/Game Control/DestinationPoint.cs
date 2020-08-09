using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPoint : MonoBehaviour {

    GameController gc;

    // Start is called before the first frame update
    void Start() {
        gc = GameController.instance;
    }

    // Update is called once per frame
    void Update() {

    }


    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.GetComponent<GlobalPlayerController>()) {
            gc.AddPoint(coll.gameObject.GetComponent<GlobalPlayerController>(), 1);
            gc.SetNewDestination();

            Debug.Log("+1 for :" + coll.gameObject);
            
            
        }
    }
}
