using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugWindow : MonoBehaviour
{

    public GlobalPlayerController gpc;
    Rigidbody rb;
    GrindPlayerController grind;
    
    PlayerHealth playerHealth;
    public TMP_Text totalSpeed;
    public TMP_Text hSpeed;
    public TMP_Text vSpeed;
    public TMP_Text combo;
    public TMP_Text health;

    void Start() {
        rb = gpc.gameObject.GetComponent<Rigidbody>();
        playerHealth = gpc.gameObject.GetComponent<PlayerHealth>();
        grind = gpc.gameObject.GetComponent<GrindPlayerController>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vel = rb.velocity;

        if(grind.enabled) {
            totalSpeed.text = "Total Speed: " + grind.GetCurrentGrindSpeed().ToString("0.000");
        } else {
            totalSpeed.text = "Total Speed: " + vel.magnitude.ToString("0.000");
        }
        
        vSpeed.text = "Vertical Speed: " + vel.y.ToString("0.000");;
        vel.y = 0;
        hSpeed.text = "Horizontal Speed: " + vel.magnitude.ToString("0.000");;
        combo.text = "Combo Multiplier: " + gpc.currentSpeedMultiplier + "x";
        health.text = "Health: " + playerHealth.currentHealth.ToString("0.0");;
    }
}
