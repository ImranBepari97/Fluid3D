using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Checkpoint : NetworkBehaviour
{

    public static Dictionary<GameObject, Vector3> playerCheckpointMap = new Dictionary<GameObject, Vector3>();
    Vector3 checkpointLocation;
    // Start is called before the first frame update
    void Start()
    {
        checkpointLocation = transform.GetChild(0).transform.position;
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<GlobalPlayerController>()) {
            if(playerCheckpointMap.ContainsKey(other.gameObject)) {
                playerCheckpointMap.Remove(other.gameObject);
            }

            playerCheckpointMap.Add(other.gameObject, checkpointLocation);

            
        }
    }
}
