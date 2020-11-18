using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank_Movement_Improved : MonoBehaviour {
    public WheelCollider[] wheelColliders = new WheelCollider[16];  // Tableau des 16 wheelColliders
    public Transform[] wheelMeshesPos = new Transform[16];          // Tableau des 16 mesh de position
    public Transform[] wheelMeshesQuat = new Transform[16];         // Tableau des 16 mesh de rotation
    public float offset = 0.01f;                                    // Offset de position Y pour laisser les mesh des roues au dessus des chenilles
    public bool isOffset = false;                                   // Activer / désactiver l'offset
    public Transform centerOfMass;                                  // Position du centre de masse
    public float enginePower = 400f;                                // Puissance du moteur marche avant / arrière
    public float turnPower = 10f;                                   // Puissance du moteur virage gauche / droite
    public float mtorque;                                           // Couple appliquer aux wheelColliders marche avant / arrière
    public float mturn;                                             // Couple appliquer aux wheelColliders virage gauche / droite
    public float minBrakeTorque = 0;                                     
    public float maxBrakeTorque = 500;
    public float actualBrakeTorque;
    public bool isSpace;                                            // Bool frein a main
    public float mRpm;                                              // Vitesse rotation roue
    Rigidbody rbody;                                                // RigidBody
    public GameObject trackLeft;                                    // Chenille gauche
    public GameObject trackRight;                                   // Chenille droite
    public Renderer lRend;                                          // Renderer chenille gauche
    public Renderer rRend;                                          // Renderer chenille droite
    public float offset1;                                           // Offset Rendere chenille gauche
    public float offset2;                                           // Offset Rendere chenille droite
    public bool isAutoStop = false;                                 // Freinage automatique
    public AudioSource EngineStatusAudio;                             // Source audio
    public AudioSource EngineRunningAudio;
    public AudioClip engineIdling;                                // Piste audio moteur idle
    public AudioClip engineStarting;
    public AudioClip engineStoping;
    public AudioClip engineDrivingR1;                               // Piste audio moteur marche
    public AudioClip engineDrivingR2;
    public float maxPitch = 1f;                                     // Pitch maximum
    public float minPitch = .5f;                                    // Pitch minimum
    public float actualPitch;                                       // Pitch actuel
    public float torque;                                            // Commande recue Z / S
    public float turnSpeed;                                         // Commande recue Q / D
    public float offsetMulitplier = 0.1f;
    public float actualTorque = 0.0f;
    public float turnCompensation1 = 1.0f;
    public float turnCompensation2 = 1.0f;
    public bool engineStarted = false;
    public string engineStartButton;
    public float engineSoundFactor = 0.0f;

    public float throttle = 0.0f;
    public float x = 0.0f;
    public bool audioSystemSwitch = false;

    void Awake(){
        rbody = GetComponent<Rigidbody>();
    }

    void Start(){
        rbody.centerOfMass = centerOfMass.localPosition;
        lRend = trackLeft.GetComponent<Renderer>();
        rRend = trackRight.GetComponent<Renderer>();
        offsetMulitplier = 0.05f;
        offsetMulitplier = offsetMulitplier / 10000;
        engineStartButton = "EngineStart";
    }

    private void FixedUpdate(){
        mRpm = wheelColliders[1].rpm;
        if (engineStarted)
        {
            float throttleInput = Input.GetAxis("Vertical") * enginePower;
            throttle += Time.deltaTime / (1.0f / (throttleInput - throttle));
            throttle = Mathf.Clamp(throttle, -1600, 1600);
            torque = throttle;

            turnSpeed = Input.GetAxis("Horizontal") * turnPower;
        }
        else
        {
            torque = 0;
            turnSpeed = 0;
        }
        mtorque = torque;
        mturn = turnSpeed;
        Move(torque);
        Turn(turnSpeed);
        Brake();
    }

    void Update()
    {
        if (Input.GetButtonDown(engineStartButton)){
            if (engineStarted == false) {
                engineStarted = true;
                StartCoroutine(playEngineSound());
            }
            else {
                EngineStatusAudio.loop = false;
                engineStarted = false;
                StopCoroutine(playEngineSound());
                EngineStatusAudio.clip = engineStoping;
                EngineStatusAudio.Play();
            }
        }
        AudioEngine();
        UpdateMeshesPositions();
        offset1 += Time.time * wheelColliders[3].rpm * offsetMulitplier * Mathf.Abs(turnCompensation1);                                 
        offset2 += Time.time * wheelColliders[11].rpm * offsetMulitplier * Mathf.Abs(turnCompensation2);
        lRend.material.mainTextureOffset = new Vector2(0, offset1);
        rRend.material.mainTextureOffset = new Vector2(0, offset2);
    }

    IEnumerator playEngineSound()
    {
        EngineStatusAudio.clip = engineStarting;
        EngineStatusAudio.Play();
        yield return new WaitForSeconds(EngineStatusAudio.clip.length);
        if(engineStarted == true) {
            EngineStatusAudio.loop = true;
            EngineStatusAudio.clip = engineIdling;
            EngineStatusAudio.Play();
        }
    }

    void Move(float motorTorque){
        if (motorTorque >= 0){
            turnCompensation1 = 1.0f;
            turnCompensation2 = 1.0f;
        }
        else
        {
            turnCompensation1 = -1.0f;
            turnCompensation2 = -1.0f;
        }
        for (int i = 0; i < 16; i++)
        {
            if (i != 0 && i != 7 && i != 8 && i != 15)
            {
                wheelColliders[i].motorTorque = motorTorque;
            }
        }
        /*if(motorTorque == 0){
            for (int i = 0; i < 16; i++){
                wheelColliders[i].brakeTorque = 5000f;
            }
        }
        else{
            for (int i = 0; i < 16; i++){
                wheelColliders[i].brakeTorque = 0;
            }
        }*/
    }

    void Turn(float motorTurnTorque){
        /*float actualTorque = wheelColliders[4].motorTorque;
        if (motorTurnTorque < 0){
            for (int i = 0; i < 7; i++){
                wheelColliders[i].motorTorque = -(motorTurnTorque);          // VIRAGE PROGRESSIF
            }
            for (int i = 8; i < 16; i++){

                wheelColliders[i].motorTorque = motorTurnTorque;
            }
        }
        if (motorTurnTorque > 0){
            for (int i = 8; i < 16; i++){
                wheelColliders[i].motorTorque = motorTurnTorque;
            }
            for (int i = 0; i < 7; i++){
                wheelColliders[i].motorTorque = -(motorTurnTorque);
            }
        }*/
        ///////////////////////////////////////////////////////////////////
        actualTorque = motorTurnTorque + torque;
        if (motorTurnTorque < 0)                                                    //VIRAGE GAUCHE
        {
            if (torque >= 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    wheelColliders[i].motorTorque = torque - motorTurnTorque * turnCompensation1;       //DROITE
                }
                for (int i = 8; i < 16; i++)
                {
                    wheelColliders[i].motorTorque = torque * turnCompensation1/*motorTurnTorque*/;     //GAUCHE
                }
            }
            else if(torque < 1) {                                     
                for (int i = 0; i < 7; i++)
                {
                    wheelColliders[i].motorTorque = torque - motorTurnTorque * turnCompensation1;       //DROITE
                }
                for (int i = 8; i < 16; i++)
                {             
                    wheelColliders[i].motorTorque = torque + motorTurnTorque * turnCompensation1;       //GAUCHE
                }
            }
        }
        else if (motorTurnTorque > 0)                                               //VIRAGE DROITE
        {
            if (torque >= 1)                                           
            {
                for (int i = 8; i < 16; i++)
                {
                    wheelColliders[i].motorTorque = torque * turnCompensation1/*motorTurnTorque*/;     //DROITE
                }
                for (int i = 0; i < 7; i++)
                {
                    wheelColliders[i].motorTorque = torque - motorTurnTorque * turnCompensation1;    //GAUCHE
                }
            }
            else if (torque < 1)                                      
            {
                for (int i = 0; i < 7; i++)
                {
                    wheelColliders[i].motorTorque = torque - motorTurnTorque * turnCompensation1;       //DROITE
                }
                for (int i = 8; i < 16; i++)
                {
                    wheelColliders[i].motorTorque = torque + motorTurnTorque * turnCompensation1;    //GAUCHE
                }
            }
        }
        ///////////////////////////////////////////////////////////////////
    }

    void UpdateMeshesPositions(){
        for(int i = 0; i < 16; i++){
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);                                  // SMOOTHING DE LA POSITION DES ROUES
            //Vector3 targetPosition = pos;
            if (i != 0 && i != 7 && i != 8 && i != 15){
                wheelMeshesPos[i].position = pos;
                //wheelMeshesPos[i].position = new Vector3(Mathf.Lerp(wheelMeshesPos[i].position.x, pos.x, t), Mathf.Lerp(wheelMeshesPos[i].position.y, pos.y, t), Mathf.Lerp(wheelMeshesPos[i].position.z, pos.z, t));
                //wheelMeshesPos[i].position = Vector3.SmoothDamp(wheelMeshesPos[i].position, targetPosition, ref velocity, smoothTime);
                wheelMeshesPos[i].position = wheelMeshesPos[i].position - new Vector3(0, offset, 0);
                wheelMeshesQuat[i].rotation = quat;
            }
            else{
                wheelMeshesPos[i].position = pos;
                //wheelMeshesPos[i].position = new Vector3(Mathf.Lerp(wheelMeshesPos[i].position.x, pos.x, t), Mathf.Lerp(wheelMeshesPos[i].position.y, pos.y, t), Mathf.Lerp(wheelMeshesPos[i].position.z, pos.z, t));
                //wheelMeshesPos[i].position = Vector3.SmoothDamp(wheelMeshesPos[i].position, targetPosition, ref velocity, smoothTime);
                wheelMeshesQuat[0].rotation = wheelMeshesQuat[1].rotation;
                wheelMeshesQuat[7].rotation = wheelMeshesQuat[1].rotation;
                wheelMeshesQuat[8].rotation = wheelMeshesQuat[9].rotation;
                wheelMeshesQuat[15].rotation = wheelMeshesQuat[9].rotation;
                if (isOffset){
                    wheelMeshesPos[i].position = wheelMeshesPos[i].position - new Vector3(0, offset, 0);
                }
            }
            wheelMeshesQuat[i].rotation = wheelMeshesQuat[i].rotation * new Quaternion(0, 0, -180, 1);
        }
    }

    /*private void AudioEngine(){                                                                 // REECRIRE LE MOTEUR AUDIO -> PITCH PROGRESSIF EN 2-3 ETAPES MAX
        // If there is no input (the tank is stationary)...
        if (torque < 0.1f && turnSpeed < 0.1f){
            // ... and if the audio source is currently playing the driving clip...
            if (m_RunningAudio.clip == m_EngineDrivingR1){
                // ... change the clip to idling and play it.
                m_RunningAudio.clip = m_EngineIdling;
                //m_RunningAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_RunningAudio.Play();
            }
        }
        else{
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_RunningAudio.clip == m_EngineIdling){
                // ... change the clip to driving and play.
                m_RunningAudio.clip = m_EngineDrivingR1;
                m_RunningAudio.Play();
            }
        }
    }*/

    void AudioEngine()
    {
        //-2.0504*x^3+3.1853*x^2-0.1326*x+0.0052
        engineSoundFactor = rbody.velocity.magnitude/6.0f;
        if (audioSystemSwitch)
        {
            x = engineSoundFactor;
        }
        else
        {
            x = torque / 1600;
        }

        if ((x > 0.01f || turnSpeed > 0.01f) && engineStarted)
        {
            if (!EngineRunningAudio.isPlaying)
            {
                EngineRunningAudio.Play();
            }
            //EngineRunningAudio.pitch = -2.0504f * Mathf.Pow(x, 3.0f) + 3.1853f * Mathf.Pow(x, 2.0f) - 0.1326f * x + 0.0052f;
            EngineRunningAudio.pitch = 0.286f * x + 0.7143f; 
            EngineRunningAudio.volume = 19.075f * Mathf.Pow(x, 5.0f) - 57.543f * Mathf.Pow(x, 4.0f) + 65.445f * Mathf.Pow(x, 3.0f) - 34.588f * Mathf.Pow(x, 2.0f) + 8.6212f * x;
            EngineStatusAudio.volume = 1 - EngineRunningAudio.volume;
        }
        else
        {
            EngineStatusAudio.volume = 1;
            EngineRunningAudio.Stop();
        }
    }

    void Brake()
    {
        isSpace = Input.GetKey(KeyCode.B);
        if (isSpace || (mtorque == 0 && mturn == 0) && isAutoStop)
        {
            actualBrakeTorque = maxBrakeTorque;
            for (int i = 0; i < 16; i++)
            {
                wheelColliders[i].brakeTorque = actualBrakeTorque;
            }
        }
        else
        {
            actualBrakeTorque = minBrakeTorque;
            for (int i = 0; i < 16; i++)
            {
                wheelColliders[i].brakeTorque = actualBrakeTorque;
            }
        }
    }
}