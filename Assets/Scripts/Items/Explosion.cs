using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Explosion : NetworkBehaviour
{
    [SerializeField] float damagePower = 100f;
    [SerializeField] float time = 2f;

    public override void OnStartServer() {
        base.OnStartServer();
        StartCoroutine(DestroyCooldown());

    }

    IEnumerator DestroyCooldown() {
        yield return new WaitForSeconds(time);
        NetworkServer.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(!isServer) {
            return;
        }

        if(other.gameObject.GetComponent<GamePlayerEntity>()) {
            NetworkIdentity identityOfPlayer = other.gameObject.GetComponent<NetworkIdentity>();
            RaycastHit hit;
            LayerMask mask = 1 << LayerMask.NameToLayer("Player");
            mask = ~mask;

            if(Physics.Linecast(transform.position, other.transform.position, out hit, mask)) {
                if(hit.collider != other) {
                    return; // if theres something in the way, dont hit
                }
            }

            if(identityOfPlayer.connectionToClient != connectionToClient) {
                other.gameObject.GetComponent<PlayerHealth>().Damage(damagePower);
            }

        }
    }
}
