using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    private float typingSpeed;
    [SerializeField] float timeBetweenTexts = 5f;
    [SerializeField] List<string> tutorialTexts;
    [SerializeField] TextMeshProUGUI tutorialTextMeshPro;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        StartCoroutine(TypeLines());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.HasKey("TextVelocity"))
        {
            int dbTextVelocity = PlayerPrefs.GetInt("TextVelocity");
            if (dbTextVelocity == 0)
            {
                typingSpeed = 0.1f;
            }
            else if (dbTextVelocity == 1)
            {
                typingSpeed = 0.05f;
            }
            else if (dbTextVelocity == 2)
            {
                typingSpeed = 0.01f;
            }
        }
        else
        {
            typingSpeed = 0.05f;
        }
    }

    public IEnumerator TypeLines()
    {
        yield return new WaitForSeconds(GetAnimationDuration("Open"));
        foreach (string tutorialText in tutorialTexts)
        {
            yield return TypeLineCoroutine(tutorialText);
        }
        tutorialTextMeshPro.text = "";
        animator.Play("Close");
        yield return new WaitForSeconds(GetAnimationDuration("Close"));
        gameObject.SetActive(false);
    }

    IEnumerator TypeLineCoroutine(string tutorialText)
    {
        tutorialTextMeshPro.text = "";
        foreach (char letter in tutorialText.ToCharArray())
        {
            tutorialTextMeshPro.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(timeBetweenTexts);
    }

    float GetAnimationDuration(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                return ac.animationClips[i].length;
            }
        }

        Debug.LogError("La animación con nombre '" + animationName + "' no fue encontrada.");
        return 0;
    }
}
