using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{

    GlobalPlayerController gpc;
    WallPlayerController wpc;
    Rigidbody rb;
    CapsuleCollider capsuleCollider;
    public GameObject model;
    public GameObject ragdollPrefab;

    public Vector3 deathVelocity;

    public float respawnTime = 5f;

    [SyncVar]
    public float currentHealth;
    public float maxHealth = 100f;

    //[SyncVar (hook = nameof(Damage))]
    float timeSinceLastFallDamage;
    public float fallDamageThresholdVelocity = 40f;

    [Header("Health Regen Properties")]
    public bool canRegen = true;
    public float healthRegenTime = 5f;
    public float regenRate = 25f;
    
    // Start is called before the first frame update
    void Awake()
    {
        gpc = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        wpc = GetComponent<WallPlayerController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentHealth = maxHealth;
        timeSinceLastFallDamage = healthRegenTime;

        Checkpoint.playerCheckpointMap[this.gameObject] = transform.position;
        deathVelocity = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!canRegen) {
            return;
        }
        
        timeSinceLastFallDamage += Time.deltaTime;
        if(timeSinceLastFallDamage > healthRegenTime && currentHealth < maxHealth) {
            currentHealth += regenRate * Time.deltaTime;
            if(currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }
    }

    public void Die() {
        HandleRagdoll();
        gpc.DisableAllControls();
        gpc.enabled = false;
        wpc.wallsCollidingWith.Clear();

        model.SetActive(false);

        rb.useGravity = false;
        capsuleCollider.enabled = false;

        StartCoroutine(RespawnCoroutine());

    }

    void HandleRagdoll() {
        Vector3 dollPos = new Vector3(transform.position.x,  transform.position.y - (capsuleCollider.height / 2),  transform.position.z);
        GameObject ragdoll = Instantiate(ragdollPrefab, dollPos , transform.rotation);
        
        //Debug.Log("curVelMag =  " + rb.velocity.magnitude);

        if(deathVelocity == new Vector3(0,0,0)) {
            deathVelocity = rb.velocity;
        } else {
            deathVelocity = -deathVelocity / 2f;
        }

        Rigidbody[] rbs = ragdoll.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody r in rbs) {

            r.velocity = deathVelocity * Random.Range(0.9f, 1.1f);
        }

        Destroy(ragdoll, respawnTime);
    }


    [Command]
    public void CmdRespawn() {
        RpcRespawn();
        //GetComponent<GamePlayerEntity>().TargetSetPosition(this.connectionToClient, Checkpoint.playerCheckpointMap[this.gameObject], transform.rotation);
    }

    [ClientRpc]
    public void RpcRespawn() {
        gpc.enabled = true;
        transform.position = Checkpoint.playerCheckpointMap[this.gameObject];
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = true;
        capsuleCollider.enabled = true;

        currentHealth = maxHealth;
        gpc.EnableDefaultControls();
        model.SetActive(true);
    }
    public void HandleFallDamage(float floorTouchVelocity) {

        if(floorTouchVelocity > fallDamageThresholdVelocity) {
            //Debug.Log("fall damage time");
            StartCoroutine(FallDamageCoroutine(floorTouchVelocity));
        }

        floorTouchVelocity = 0; 
    }

    public void Damage(float damageDealt) {
        currentHealth -= damageDealt;
        if(currentHealth < 0) {
            currentHealth = 0;
            Die();
        }
    }

    public IEnumerator FallDamageCoroutine(float velocity) {
        yield return new WaitForSeconds(0.066f);
        if(!(gpc.floorNormal != new Vector3(0,1,0) && gpc.recentAction == RecentActionType.Slide)) {
            //Debug.Log("fall damage at " + velocity + " is " + PlayerAnimator.RangeRemap(velocity, 40f, 53f, 0f, 100f));
            Damage(PlayerAnimator.RangeRemap(velocity, 40f, 53f, 0f, 100f));
        
            timeSinceLastFallDamage = 0f;
        }
    }

    public IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(respawnTime);
        CmdRespawn();
    }
}
