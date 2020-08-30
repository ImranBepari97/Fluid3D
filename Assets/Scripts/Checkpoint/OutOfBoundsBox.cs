using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsBox : MonoBehaviour
{

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<GlobalPlayerController>()) {
            Rigidbody pRb = other.gameObject.GetComponent<Rigidbody>();
            pRb.position = Checkpoint.playerCheckpointMap[other.gameObject].position;
            pRb.velocity = new Vector3(0, 0, 0);
        }
    }
}
