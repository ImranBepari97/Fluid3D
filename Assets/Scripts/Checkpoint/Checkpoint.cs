using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public static Dictionary<GameObject, Transform> playerCheckpointMap = new Dictionary<GameObject, Transform>();
    Transform checkpointLocation;
    // Start is called before the first frame update
    void Start()
    {
        checkpointLocation = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        
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
