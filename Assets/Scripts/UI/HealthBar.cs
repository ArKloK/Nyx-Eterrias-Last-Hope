using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f); // 1f is the max value of the slider
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue); // normalizedValue is the value of the slider between 0 and 1
    }

    //Create a IEnumerator to animate the health bar
    public IEnumerator SetHealthAnimated(int health)
    {
        float currentHealth = slider.value;
        float targetHealth = health;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            slider.value = Mathf.Lerp(currentHealth, targetHealth, t);
            fill.color = gradient.Evaluate(slider.normalizedValue);
            yield return null;
        }
    }
}
