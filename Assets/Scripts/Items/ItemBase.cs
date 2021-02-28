using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Mirror;

public class ItemBase : NetworkBehaviour {
    [SyncVar] public NetworkIdentity owner;
    public bool requiresAim;
    [SerializeField] protected int numberOfUses;

    protected Transform lookAtTarget;

    ParentConstraint constraint;

    public override void OnStartClient() {
        if(!isServer) {
            PositionSelf();
        }
    }

    public override void OnStartServer() {
        PositionSelf();
    }

    public void PositionSelf() {
        lookAtTarget = GameObject.FindGameObjectWithTag("LookAtTarget").transform;
        constraint = GetComponent<ParentConstraint>();

        if(owner != null) {
            ConstraintSource cs = new ConstraintSource();
            cs.sourceTransform = owner.gameObject.GetComponent<ItemController>().useWeaponBone;
            cs.weight = 1;
            ConstraintSource cs1 = new ConstraintSource();
            cs1.sourceTransform =  owner.gameObject.GetComponent<ItemController>().restWeaponBone;
            
            cs1.weight = 0;
            constraint.AddSource(cs);
            constraint.AddSource(cs1);
            constraint.constraintActive = true;
        }
    }

    public void TryUse() {
        Debug.Log("Calling command");
        CmdUse();
    }

    [Command]
    public void CmdUse(NetworkConnectionToClient sender = null) {
        Debug.Log("Reached server:" + sender);
        if(sender.clientOwnedObjects.Contains(this.netIdentity)) {
            Debug.Log("Using");
            Use();
        }
    }

    [Server]
    public virtual void Use() {
        //Put use mechanics here
        numberOfUses--;
        if (numberOfUses <= 0) {
            Destroy(this.gameObject);
        }
    }
}
