using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour {

    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public string m_FireButton;                 // The input axis that is used for launching shells.
    public Transform ExplosionTransform;
    public Vector3 VExplosionForce;
    public Rigidbody TankRigidbody;
    public float reloadTime = 4.0f;
    public bool canFire = false;
    public float startTime;
    public float actualTime;
    public float reloadedTime;
    public Slider ammoSlider;
    public Text ammoText;
    public int ammoNumber = 0;

    private void Start()
    {
        m_FireButton = "Fire";
        startTime = Time.time;
        ammoSlider.maxValue = reloadTime;
    }

    private void Update()
    {
        actualTime = Time.time - startTime;
        ammoSlider.value = reloadTime - (reloadedTime - actualTime);
        ammoText.text = ammoNumber.ToString();
        Reloading();
        if (Input.GetButtonDown(m_FireButton) && canFire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_MaxLaunchForce * m_FireTransform.forward;
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        TankRigidbody.AddForceAtPosition(VExplosionForce, ExplosionTransform.position);
        reloadedTime = actualTime + reloadTime;
        ammoNumber = 0;
    }

    private void Reloading()
    {
        if(actualTime < reloadedTime)
        {
            canFire = false;
        }
        else
        {
            canFire = true;
            ammoNumber = 1;
        }
    }
}
