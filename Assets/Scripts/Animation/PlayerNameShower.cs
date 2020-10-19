using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameShower : MonoBehaviour {
    GamePlayerEntity gpe;
    [SerializeField] TMP_Text playerDisplayName;
    [SerializeField] float distanceToSeeTagFull = 15f;
    [SerializeField] float distanceToSeeTagMin = 20f;

    // Start is called before the first frame update
    void Start() {
        gpe = GetComponentInParent<GamePlayerEntity>();
        playerDisplayName.text = gpe.displayName;

        if (gpe.hasAuthority) { //we dont want to see our own name
            this.gameObject.SetActive(false);
        }
    }

    void Update() {
        float distToCamera = Vector3.Distance(this.transform.position, Camera.main.transform.position);

        if ((distToCamera < distanceToSeeTagFull)) {
            playerDisplayName.alpha = 1;
            transform.LookAt(Camera.main.transform);
        } else if (distToCamera < distanceToSeeTagMin) {
            playerDisplayName.alpha = PlayerAnimator.RangeRemap(distToCamera, distanceToSeeTagMin, distanceToSeeTagFull, 0, 1);
            transform.LookAt(Camera.main.transform);
        } else {
            playerDisplayName.alpha = 0;
        }
    }
}
