using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;
    public delegate void ExperienceChangedHandler(int amount);
    public static event ExperienceChangedHandler OnExperienceChanged;

    //Singleton check
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddExperience(int amount)
    {
        if (OnExperienceChanged != null)
        {
            OnExperienceChanged(amount);
        }
    }
}
