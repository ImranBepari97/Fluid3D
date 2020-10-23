using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SimpleProjectile : ItemBase
{


    public GameObject projectileToSpawn;



    [Server]
    public override void Use() {

        Instantiate(projectileToSpawn, transform.position + (transform.forward * 1f), Quaternion.LookRotation(lookAtTarget.position - transform.position));
        owner.gameObject.GetComponent<Rigidbody>().AddForce( (transform.position - lookAtTarget.position).normalized * 50f, ForceMode.Impulse);

        //Spawn Rocket
        base.Use();
    }


}
