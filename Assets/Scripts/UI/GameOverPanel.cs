using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public static event Action OnRestartButtonClicked;

    public void RestartButtonClicked()
    {
        OnRestartButtonClicked?.Invoke();
    }
}
