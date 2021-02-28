using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Mirror;

public class BasicRocket : NetworkBehaviour
{
    public float speed = 50f;
    VisualEffect emit;
    public GameObject model;
    [Tooltip("How long the rocket will live before it's despawned.")] public float lifetime = 15f;

    public NetworkIdentity owner;

    public GameObject explosion;


    void Awake() {
        emit = GetComponentInChildren<VisualEffect>();
    }

    public override void OnStartServer() {
        base.OnStartServer();
        owner = connectionToClient.identity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (transform.forward * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;
        if(lifetime < 0) {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {
        emit.Stop();

        Destroy(emit.gameObject, 2f);
        Destroy(model);
        transform.DetachChildren();

        if(isServer) {
            Debug.Log("Spawning explosion");
            NetworkServer.Destroy(this.gameObject);

            GameObject explosionGameObject = Instantiate(explosion, transform.position, Quaternion.identity);
            NetworkServer.Spawn(explosionGameObject, owner.connectionToClient);
        }
        
    }
}
