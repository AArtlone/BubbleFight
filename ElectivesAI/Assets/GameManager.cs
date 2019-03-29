using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Beginning");
    }

    public void DestroyTank(GameObject tank)
    {
        ParticlesManager.Instance.CreateTankExplosion(tank.transform);
        AudioManager.Instance.PlayExplosion();
        Destroy(tank.transform.parent.gameObject);
    }
}
