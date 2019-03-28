using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public static ParticlesManager Instance;
    public GameObject ExplosionParticles;
    public GameObject BigExplosionParticles;

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
        AudioManager.Instance.PlayHit();

        Destroy(obj.gameObject);
        Destroy(explosion, 1f);
    }

    public void CreateTankExplosion(Transform obj)
    {
        GameObject explosion = Instantiate(BigExplosionParticles, obj.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        AudioManager.Instance.PlayHit();
        Destroy(explosion, 1f);
    }
}
