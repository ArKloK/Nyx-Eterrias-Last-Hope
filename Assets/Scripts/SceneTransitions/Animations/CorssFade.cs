using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class CorssFade : SceneTransition
{
    public CanvasGroup crossFade;
    public float duration;

    public override IEnumerator AnimateTransitionIn()
    {
        if (crossFade != null)
        {
            var tweener = crossFade.DOFade(1f, duration);
            yield return tweener.WaitForCompletion();
        }
        else
        {
            Debug.LogWarning("CanvasGroup is null, cannot perform transition in.");
        }
    }

    public override IEnumerator AnimateTransitionOut()
    {
        if (crossFade != null)
        {
            var tweener = crossFade.DOFade(0f, duration);
            yield return tweener.WaitForCompletion();
        }
        else
        {
            Debug.LogWarning("CanvasGroup is null, cannot perform transition out.");
        }
    }

    void OnDestroy()
    {
        // Cancela todos los tweens asociados con el objeto si el objeto se destruye
        if (crossFade != null)
        {
            DOTween.Kill(crossFade);
        }
    }
}
