using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemSpawner : NetworkBehaviour {

    public float delayToGiveItem;

    public List<ItemBase> possibleItems;

    MeshRenderer renderer;

    public float respawnTime = 3f;

    [SyncVar]
    float timeTillActive = -1f;


    private void Awake() {
        renderer = GetComponent<MeshRenderer>();
    }
    private void Update() {
        if (timeTillActive >= 0) {
            renderer.enabled = false;
            timeTillActive -= Time.deltaTime;
        } else {
            renderer.enabled = true;
        }

        transform.Rotate(0, 100 * Time.deltaTime, 50 * Time.deltaTime);
    }

    [Server]
    void GiveItem(NetworkIdentity newOwner) {
        StartCoroutine(GiveItemRoutine(newOwner));
    }

    IEnumerator GiveItemRoutine(NetworkIdentity newOwner) {
        yield return new WaitForSeconds(delayToGiveItem);
        ItemController player = newOwner.gameObject.GetComponent<ItemController>();
        if (player.currentItem == null) {
            if (timeTillActive < 0) {
                int random = Random.Range(0, possibleItems.Count);
                GameObject newItem = Instantiate(possibleItems[random].gameObject);
                ItemBase newBase = newItem.GetComponent<ItemBase>();

                newBase.owner = newOwner;
                player.currentItem = newBase;

                NetworkServer.Spawn(newItem, newOwner.connectionToClient);
                timeTillActive = respawnTime;
            } else {
                Debug.Log("Someone tried to call item giving on the server when the CD isn't done");
            }
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (!isServer) {
            return;
        }

        ItemController ite;
        if ((ite = other.GetComponent<ItemController>()) && timeTillActive < 0) {
            GiveItem(other.GetComponent<NetworkIdentity>());
        }
    }


}
