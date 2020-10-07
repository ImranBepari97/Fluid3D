using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkCapsuleCollider : NetworkBehaviour {

    static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkCapsuleCollider));

    [Header("Settings")]
    [SerializeField] internal CapsuleCollider target = null;

    [Tooltip("Set to true if moves come from owner client, set to false if moves always come from server")]
    [SerializeField] bool clientAuthority = false;

    [Header("Capsule Height")]

    [Tooltip("Syncs capsule height every SyncInterval")]
    [SerializeField] bool syncHeight = true;

    [Tooltip("Only Syncs Value if distance between previous and current is great than sensitivity")]
    [SerializeField] float heightSensitivity = 0.1f;

    [Header("Capsule Radius")]

    [Tooltip("Syncs capsule radius every SyncInterval")]
    [SerializeField] bool syncRadius = true;

    [Tooltip("Only Syncs Value if distance between previous and current is great than sensitivity")]
    [SerializeField] float radiusSensitivity = 0.1f;


    [Header("Capsule Center")]

    [Tooltip("Syncs capsule center every SyncInterval")]
    [SerializeField] bool syncCenter = true;

    [Tooltip("Only Syncs Value if distance between previous and current is great than sensitivity")]
    [SerializeField] float centerSensitivity = 0.1f;

    [Header("Capsule Direction")]

    [Tooltip("Syncs capsule direction every SyncInterval")]
    [SerializeField] bool syncDirection = true;

    /// <summary>
    /// Values sent on client with authoirty after they are sent to the server
    /// </summary>
    readonly ClientSyncState previousValue = new ClientSyncState();


    void OnValidate() {
        if (target == null) {
            target = GetComponent<CapsuleCollider>();
        }
    }


    [SyncVar(hook = nameof(OnHeightChanged))]
    float height;

    [SyncVar(hook = nameof(OnRadiusChanged))]
    float radius;

    [SyncVar(hook = nameof(OnCenterChanged))]
    Vector3 center;

    [SyncVar(hook = nameof(OnDirectionChanged))]
    int direction;

    /// <summary>
    /// Ignore value if is host or client with Authority
    /// </summary>
    /// <returns></returns>
    bool IgnoreSync => isServer || ClientWithAuthority;

    bool ClientWithAuthority => clientAuthority && hasAuthority;

    void OnCenterChanged(Vector3 _, Vector3 newValue) {
        if (IgnoreSync)
            return;

        target.center = newValue;
    }

    void OnHeightChanged(float _, float newValue) {
        if (IgnoreSync)
            return;

        target.height = newValue;
    }

    void OnRadiusChanged(float _, float newValue) {
        if (IgnoreSync)
            return;

        target.radius = newValue;
    }

    void OnDirectionChanged(int _, int newValue) {
        if (IgnoreSync)
            return;

        target.direction = newValue;
    }

    internal void Update() {
        if (isServer) {
            SyncToClients();
        } else if (ClientWithAuthority) {
            SendToServer();
        }
    }

    [Server]
    void SyncToClients() {
        float currentHeight = syncHeight ? target.height : default;
        float currentRadius = syncRadius ? target.radius : default;
        Vector3 currentCenter = syncCenter ? target.center : default;


        bool centerChanged = syncCenter && ((previousValue.center - currentCenter).sqrMagnitude > centerSensitivity * centerSensitivity);
        bool heightChanged = syncHeight && Mathf.Abs(previousValue.height - currentHeight) > heightSensitivity;
        bool radiusChanged = syncRadius && Mathf.Abs(previousValue.radius - currentRadius) > radiusSensitivity;

        if (centerChanged) {
            center = currentCenter;
            previousValue.center = currentCenter;
        }

        if (heightChanged) {
            height = currentHeight;
            previousValue.height = currentHeight;
        }

        if (radiusChanged) {
            radius = currentRadius;
            previousValue.radius = currentRadius;
        }

        if (previousValue.direction != target.direction) {
            direction = target.direction;
            previousValue.direction = target.direction;
        }
    }

    [Client]
    void SendSize() {
        float now = Time.time;
        if (now < previousValue.nextSyncTime)
            return;

        float currentHeight = syncHeight ? target.height : default;
        float currentRadius = syncRadius ? target.radius : default;
        Vector3 currentCenter = syncCenter ? target.center : default;


        bool centerChanged = syncCenter && ((previousValue.center - currentCenter).sqrMagnitude > centerSensitivity * centerSensitivity);
        bool heightChanged = syncHeight && Mathf.Abs(previousValue.height - currentHeight) > heightSensitivity;
        bool radiusChanged = syncRadius && Mathf.Abs(previousValue.radius - currentRadius) > radiusSensitivity;

        if (centerChanged) {
            CmdSendCenter(currentCenter);
        }

        if (heightChanged) {
            CmdSendHeight(currentHeight);
        }

        if (radiusChanged) {
            CmdSendHeight(currentHeight);
        }

        if (previousValue.direction != target.direction) {
            CmdSendDirection(target.direction);
            previousValue.direction = target.direction;
        }

        if (heightChanged || radiusChanged || centerChanged) {
            previousValue.nextSyncTime = now + syncInterval;
        }
    }


    [Client]
    void SendCapsuleDirection() {
        // These shouldn't change often so it is ok to send in their own Command
        if (previousValue.direction != target.direction) {
            CmdSendDirection(target.direction);
            previousValue.direction = target.direction;
        }
    }

    [Client]
    void SendToServer() {
        if (!hasAuthority) {
            logger.LogWarning("SendToServer called without authority");
            return;
        }

        SendSize();
    }

    [Command]
    void CmdSendHeight(float height) {
        // Ignore messages from client if not in client authority mode
        if (!clientAuthority)
            return;

        this.height = height;
        target.height = height;
    }

    [Command]
    void CmdSendRadius(float radius) {
        // Ignore messages from client if not in client authority mode
        if (!clientAuthority)
            return;

        this.radius = radius;
        target.radius = radius;
    }


    [Command]
    void CmdSendDirection(int direction) {
        if (!clientAuthority)
            return;

        this.direction = direction;
        target.direction = direction;
    }

    [Command]
    void CmdSendCenter(Vector3 center) {
        if (!clientAuthority)
            return;

        this.center = center;
        target.center = center;
    }


    public class ClientSyncState {
        public float nextSyncTime;
        public float height;
        public float radius;
        public Vector3 center;
        public int direction;

    }
}
