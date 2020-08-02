using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPoint : MonoBehaviour {

    GameController gc;

    // Start is called before the first frame update
    void Start() {
        gc = GameObject.FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update() {

    }


    void OnTriggerEnter(Collider coll) {
        if(this.enabled && coll.gameObject.GetComponent<GlobalPlayerController>()) {
            gc.AddPoint(coll.gameObject, 1);
            gc.SetNewDestination();
        }
    }
}
