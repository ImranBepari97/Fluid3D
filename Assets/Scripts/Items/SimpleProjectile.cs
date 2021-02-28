using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SimpleProjectile : ItemBase
{

    public Transform shootPoint;
    public GameObject projectileToSpawn;

    [Server]
    public override void Use() {

        GameObject projectile = Instantiate(projectileToSpawn, shootPoint.position + (shootPoint.forward * 1.5f), Quaternion.LookRotation(lookAtTarget.position - transform.position));
        NetworkServer.Spawn(projectile, netIdentity.connectionToClient);

        netIdentity.connectionToClient.identity.GetComponent<GamePlayerEntity>().RpcAddForce((transform.position - lookAtTarget.position).normalized * 50f, ForceMode.Impulse);
        
        //Spawn Rocket
        base.Use();
    }


}
