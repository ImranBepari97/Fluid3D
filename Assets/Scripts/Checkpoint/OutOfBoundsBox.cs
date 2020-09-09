using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsBox : MonoBehaviour
{

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<GlobalPlayerController>()) {
            PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
            ph.deathVelocity = new Vector3(0,0,0);
            ph.Die();
        }
    }
}
