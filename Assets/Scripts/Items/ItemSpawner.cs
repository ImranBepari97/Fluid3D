using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemSpawner : NetworkBehaviour {

    public float delayToGiveItem;

    public List<ItemBase> possibleItems;

    MeshRenderer meshRenderer;

    public float respawnTime = 3f;

    [SyncVar]
    float timeTillActive = -1f;


    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Update() {
        if (timeTillActive >= 0) {
            meshRenderer.enabled = false;
            timeTillActive -= Time.deltaTime;
        } else {
            meshRenderer.enabled = true;
        }

        transform.Rotate(0, 100 * Time.deltaTime, 50 * Time.deltaTime);
    }

    [Server]
    void GiveItem(NetworkIdentity newOwner) {
        StartCoroutine(GiveItemRoutine(newOwner));
    }

    IEnumerator GiveItemRoutine(NetworkIdentity newOwner) {
        ItemController player = newOwner.gameObject.GetComponent<ItemController>();
        if (player.currentItem == null) {
            if (timeTillActive < 0) {

                //trigger waiting dice roll animation here I guess

                timeTillActive = respawnTime;
                yield return new WaitForSeconds(delayToGiveItem);

                int random = Random.Range(0, possibleItems.Count);
                GameObject newItem = Instantiate(possibleItems[random].gameObject);
                ItemBase newBase = newItem.GetComponent<ItemBase>();
                newBase.owner = newOwner;

                NetworkServer.Spawn(newItem, newOwner.connectionToClient);
                player.currentItem = newItem.GetComponent<NetworkIdentity>();
                

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
