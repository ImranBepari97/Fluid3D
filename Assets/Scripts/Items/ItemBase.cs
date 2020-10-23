using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Mirror;

public class ItemBase : NetworkBehaviour {
    public NetworkIdentity owner;
    public bool requiresAim;
    [SerializeField] protected int numberOfUses;

    protected Transform lookAtTarget;

    PositionConstraint constraint;

    void Start() {
        lookAtTarget = GameObject.FindGameObjectWithTag("LookAtTarget").transform;
        constraint = GetComponent<PositionConstraint>();
        if(owner != null) {
            ConstraintSource cs = new ConstraintSource();
            cs.sourceTransform = owner.transform;
            cs.weight = 1;
            constraint.AddSource(cs);
            constraint.constraintActive = true;
        }
    }

    [Command]
    public void CmdUse(NetworkConnectionToClient sender = null) {
        if(sender.identity == owner) {
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
