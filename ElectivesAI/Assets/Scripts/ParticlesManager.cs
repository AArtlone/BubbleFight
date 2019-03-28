using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public static ParticlesManager Instance;
    public GameObject ExplosionParticles;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CreateExplosion(Transform obj)
    {
        GameObject explosion = Instantiate(ExplosionParticles, obj.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();

        Destroy(obj.gameObject);
        Destroy(explosion, 1f);
    }
}
