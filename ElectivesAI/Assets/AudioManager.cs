using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource AudioSource;

    public AudioClip ShootSound;
    public AudioClip HitSound;
    public AudioClip ExplosionSound;
    public AudioClip TreeExplosionSound;

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

    public void PlayShoot()
    {
        AudioSource.PlayOneShot(ShootSound, .1f);
    }

    public void PlayHit()
    {
        AudioSource.PlayOneShot(HitSound, .1f);
    }

    public void PlayExplosion()
    {
        AudioSource.PlayOneShot(ExplosionSound, .5f);
    }

    public void PlayTreeDestroyed()
    {
        AudioSource.PlayOneShot(TreeExplosionSound, .5f);
    }
}
