using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class HealthVignette : MonoBehaviour
{

    private Vignette vignette;
    
    private Color vignetteStartColour;
    private float vignetteStartIntensity;

    public Color vignetteDamageColour;

    public GlobalPlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        UnityEngine.Rendering.VolumeProfile volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if(!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        vignetteStartColour = vignette.color.value;
        vignetteStartIntensity = vignette.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {

        if(player.currentHealth < player.maxHealth) {
            vignette.color.value = Color.Lerp(vignetteStartColour, vignetteDamageColour, 1 - (player.currentHealth / player.maxHealth));
            vignette.intensity.value = PlayerAnimator.RangeRemap(player.currentHealth, 0f, player.maxHealth, vignetteStartIntensity + 0.25f, vignetteStartIntensity);
        } else {
            vignette.color.value = Color.Lerp(vignette.color.value, vignetteStartColour, 1f);
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignetteStartIntensity, 1f);
        }
    }
}
