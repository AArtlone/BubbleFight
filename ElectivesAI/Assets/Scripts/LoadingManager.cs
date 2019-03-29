using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    private AsyncOperation _operation;

    public TextMeshProUGUI LoadingText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadNewScene("MainGame");
    }

    public void LoadNewScene(string newScene)
    {
        //UpdateLoaderInfo(0);
        StartCoroutine(StartLoadingMainGame(newScene));
    }

    private IEnumerator StartLoadingMainGame(string sceneToLoad)
    {
        _operation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (_operation.isDone == false)
        {
            //UpdateLoaderInfo(_operation.progress);
            yield return null;
        }

        //UpdateLoaderInfo(_operation.progress);
        _operation = null;
    }

    private void UpdateLoaderInfo(float progress)
    {
        _slider.value = progress;
        LoadingText.text = "Loading | " + (int)(progress * 100f) + "%";
    }
}
