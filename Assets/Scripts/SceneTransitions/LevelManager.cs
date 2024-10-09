using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GameObject transitionsContainer;
    private SceneTransition[] transitions;
    public Slider progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
    }
    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }
    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        yield return transition.AnimateTransitionIn();

        progressBar.gameObject.SetActive(true);
        
        do{
            progressBar.value = scene.progress;
            yield return null;
        } while(scene.progress < 0.9f);
        
        scene.allowSceneActivation = true;
        progressBar.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();
    }
}
