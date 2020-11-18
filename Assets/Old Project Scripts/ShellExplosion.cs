using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour {

    public AudioSource m_ExplosionAudio;              
    public float m_MaxLifeTime = 200f;                
    public GameObject m_Shell;
    //public ParticleSystem m_ExplosionParticles;
    //public GameObject MC;
    public Light pointLight;
    public GameObject shellExplosionParticle;
    public GameObject explosionInstance;
    public bool explosed = false;

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        pointLight.enabled = false;
        if (explosed == false)
        {
            explosionInstance = Instantiate(shellExplosionParticle, transform.position, transform.rotation) as GameObject;
            explosed = true;
        }
        //MC.transform.parent = null;
        //m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        //ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        //Destroy(MC, mainModule.duration);
        Destroy(m_Shell);
        Destroy(explosionInstance, 3.0f);
        Destroy(gameObject, m_ExplosionAudio.clip.length);
    }
    /*private void OnTriggerStay(Collider other)
    {
        pointLight.enabled = false;
        GameObject explosionInstance = Instantiate(shellExplosionParticle, transform.position, transform.rotation) as GameObject;
        m_ExplosionAudio.Play();
        Destroy(m_Shell);
        Destroy(explosionInstance, 3.0f);
        Destroy(gameObject, m_ExplosionAudio.clip.length);
    }*/
}
