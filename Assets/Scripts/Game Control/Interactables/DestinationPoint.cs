using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPoint : MonoBehaviour {

    public AudioClip pointSound;
    GameControllerArena gc;

    // Start is called before the first frame update
    void Start() {
        gc = (GameControllerArena) GameControllerCommon.instance;
    }

    // Update is called once per frame
    void Update() {

    }


    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.GetComponent<GlobalPlayerController>()) {
            gc.AddPoint(coll.gameObject.GetComponent<GlobalPlayerController>(), 1);
            gc.SetNewDestination();

            AudioSource.PlayClipAtPoint(pointSound, transform.position, 0.5f);
            Debug.Log("+1 for :" + coll.gameObject);
        }
    }
}
