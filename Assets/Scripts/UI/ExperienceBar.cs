using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider;
    public void SetMaxExperience(int experience)
    {
        slider.maxValue = experience;
        slider.value = experience;
    }

    public void SetExperience(int experience)
    {
        slider.value = experience;
    }
}
