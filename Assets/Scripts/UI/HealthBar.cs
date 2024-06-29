using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
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
            yield return null;
        }
    }
}
