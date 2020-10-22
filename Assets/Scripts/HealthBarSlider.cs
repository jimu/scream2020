using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSlider : MonoBehaviour
{
    private Slider slider;
    public Gradient gradient;
    private Image fill;

    private void Awake()
    {
       // GetComponentInParent<Enemy>().OnHitsChanged += HitsChanged;
        slider = GetComponent<Slider>();
        fill = transform.Find("Fill").GetComponent<Image>();
    }
    void HitsChanged(float percent)
    {
        slider.value = percent;
        fill.color = gradient.Evaluate(percent);
    }
}
