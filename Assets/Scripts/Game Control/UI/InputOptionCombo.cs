using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputOptionCombo : MonoBehaviour
{

    public Slider slider;
    public TMP_InputField inputField;

    public Vector2 trueValueRange;
    public Vector2 perceivedValueRange;
    
    [SerializeField]
    private float trueValue;
    public float perceivedValue;

    public void SetValueFromInput(string inputValue) {
        float val = float.Parse(inputValue);

        perceivedValue = val;
        trueValue = PlayerAnimator.RangeRemap(val, perceivedValueRange.x, perceivedValueRange.y, trueValueRange.x, trueValueRange.y);
        
        slider.value = perceivedValue;
        inputField.text = val.ToString("0.00");
    }  

    public void SetValueFromSlider(float sliderValue) {
        inputField.text = sliderValue.ToString("0.00");
        slider.value = sliderValue;
        
        perceivedValue = sliderValue;
        trueValue = PlayerAnimator.RangeRemap(sliderValue, perceivedValueRange.x, perceivedValueRange.y, trueValueRange.x, trueValueRange.y);
    }  

    public void RefreshPercievedUI() {
        slider.value = perceivedValue;
        inputField.text = perceivedValue.ToString("0.00");
    }

    public void SetPerceivedValue(float newPerceivedValue) {
        perceivedValue = newPerceivedValue;
        trueValue = PlayerAnimator.RangeRemap(newPerceivedValue, perceivedValueRange.x, perceivedValueRange.y, trueValueRange.x, trueValueRange.y);
    }

    public void SetTrueValue(float newTrueValue) {
        trueValue = newTrueValue;
        perceivedValue = PlayerAnimator.RangeRemap(newTrueValue, trueValueRange.x, trueValueRange.y, perceivedValueRange.x, perceivedValueRange.y);
    }

    public float GetTrueValue() {
        return trueValue;
    }
}
